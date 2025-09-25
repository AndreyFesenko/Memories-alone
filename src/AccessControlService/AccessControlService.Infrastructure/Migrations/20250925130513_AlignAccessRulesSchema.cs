using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControlService.Infrastructure.Migrations
{
    public partial class AlignAccessRulesSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 0) Схема
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS access;");

            // 1) Колонка SubjectId -> text (на случай если осталась uuid)
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema='access' AND table_name='AccessRules'
      AND column_name='SubjectId' AND udt_name='uuid'
  ) THEN
    ALTER TABLE access.""AccessRules""
      ALTER COLUMN ""SubjectId"" TYPE text
      USING ""SubjectId""::text;
  END IF;
END $$;");

            // 2) ObjectId -> ResourceId (uuid)
            // 2.1 Если ResourceId отсутствует, а ObjectId есть — просто переименуем.
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ResourceId'
  ) AND EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ObjectId'
  ) THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN ""ObjectId"" TO ""ResourceId"";
  END IF;
END $$;");

            // 2.2 Если обе колонки присутствуют — скопируем данные и дропнем легаси ObjectId
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ResourceId'
  ) AND EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ObjectId'
  ) THEN
    UPDATE access.""AccessRules""
      SET ""ResourceId"" = COALESCE(""ResourceId"", ""ObjectId"");
    ALTER TABLE access.""AccessRules"" DROP COLUMN ""ObjectId"";
  END IF;
END $$;");

            // 3) AccessType -> Permission (text)
            // 3.1 Если Permission отсутствует, а AccessType есть — переименуем.
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='Permission'
  ) AND EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='AccessType'
  ) THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN ""AccessType"" TO ""Permission"";
  END IF;
END $$;");

            // 3.2 Если обе есть — скопируем и дропнем легаси AccessType
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='Permission'
  ) AND EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='AccessType'
  ) THEN
    UPDATE access.""AccessRules""
      SET ""Permission"" = COALESCE(""Permission"", ""AccessType"");
    ALTER TABLE access.""AccessRules"" DROP COLUMN ""AccessType"";
  END IF;
END $$;");

            // 4) Добавим отсутствующие колонки (ExpiresAt, GrantedBy) если их нет
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ExpiresAt'
  ) THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""ExpiresAt"" timestamptz NULL;
  END IF;
  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='GrantedBy'
  ) THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""GrantedBy"" uuid NULL;
  END IF;
END $$;");

            // 5) Убедимся, что типы основных колонок корректные
            migrationBuilder.Sql(@"
DO $$
BEGIN
  -- ResourceId должен быть uuid
  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_schema='access' AND table_name='AccessRules'
      AND column_name='ResourceId' AND data_type <> 'uuid'
  ) THEN
    ALTER TABLE access.""AccessRules""
      ALTER COLUMN ""ResourceId"" TYPE uuid
      USING NULLIF(""ResourceId""::text, '')::uuid;
  END IF;

  -- Permission/SubjectType/ResourceType -> text (на случай если остался varchar без нужных ограничений – не критично)
  -- Ничего не делаем специально, text нас устраивает.
END $$;");

            // 6) Проставим NOT NULL там, где это требуется моделью,
            //    предварительно заменив NULLы на дефолты.
            migrationBuilder.Sql(@"
UPDATE access.""AccessRules""
SET ""SubjectType"" = COALESCE(""SubjectType"", 'User'),
    ""ResourceType"" = COALESCE(""ResourceType"", 'Memory'),
    ""Permission""  = COALESCE(""Permission"",  'View'),
    ""CreatedAt""   = COALESCE(""CreatedAt"", NOW())
WHERE ""SubjectType"" IS NULL
   OR ""ResourceType"" IS NULL
   OR ""Permission"" IS NULL
   OR ""CreatedAt"" IS NULL;");

            migrationBuilder.Sql(@"
ALTER TABLE access.""AccessRules""
  ALTER COLUMN ""SubjectType"" SET NOT NULL,
  ALTER COLUMN ""SubjectId""   SET NOT NULL,
  ALTER COLUMN ""ResourceType"" SET NOT NULL,
  ALTER COLUMN ""ResourceId""   SET NOT NULL,
  ALTER COLUMN ""Permission""   SET NOT NULL,
  ALTER COLUMN ""CreatedAt""    SET NOT NULL;");

            // 7) Индексы и уникальность
            migrationBuilder.Sql(@"
DO $$
BEGIN
  -- композитный уникальный ключ
  IF NOT EXISTS (
    SELECT 1
    FROM pg_indexes
    WHERE schemaname='access' AND indexname='UX_AccessRules_Subject_Resource_Permission'
  ) THEN
    CREATE UNIQUE INDEX ""UX_AccessRules_Subject_Resource_Permission""
    ON access.""AccessRules"" (""SubjectType"", ""SubjectId"", ""ResourceType"", ""ResourceId"", ""Permission"");
  END IF;

  -- индексы для выборок
  IF NOT EXISTS (
    SELECT 1 FROM pg_indexes
    WHERE schemaname='access' AND indexname='IX_AccessRules_Resource'
  ) THEN
    CREATE INDEX ""IX_AccessRules_Resource""
      ON access.""AccessRules"" (""ResourceType"", ""ResourceId"");
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_indexes
    WHERE schemaname='access' AND indexname='IX_AccessRules_Subject'
  ) THEN
    CREATE INDEX ""IX_AccessRules_Subject""
      ON access.""AccessRules"" (""SubjectType"", ""SubjectId"");
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_indexes
    WHERE schemaname='access' AND indexname='IX_AccessRules_ExpiresAt'
  ) THEN
    CREATE INDEX ""IX_AccessRules_ExpiresAt""
      ON access.""AccessRules"" (""ExpiresAt"");
  END IF;
END $$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Минимальный даунгрейд: вернуть AccessType и ObjectId, если нужно
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='AccessType'
  ) THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""AccessType"" text;
    UPDATE access.""AccessRules"" SET ""AccessType"" = ""Permission"";
  END IF;

  IF NOT EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ObjectId'
  ) THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""ObjectId"" uuid;
    UPDATE access.""AccessRules"" SET ""ObjectId"" = ""ResourceId"";
  END IF;

  -- Можно вернуть тип SubjectId к uuid, если нужно:
  -- ALTER TABLE access.""AccessRules""
  --   ALTER COLUMN ""SubjectId"" TYPE uuid USING NULLIF(""SubjectId"", '')::uuid;
END $$;");

            // Индексы откатывать не обязательно (EF сам дропнет при удалении таблицы в полном даунгрейде),
            // но если нужно — можно добавить DROP INDEX ... IF EXISTS.
        }
    }
}
