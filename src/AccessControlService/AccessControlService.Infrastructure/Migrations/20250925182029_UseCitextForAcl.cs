using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControlService.Infrastructure.Migrations
{
    public partial class UseCitextForAcl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Включаем расширение citext (безопасно если уже включено)
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS citext;");

            // 2) Сносим старый уникальный индекс (если его имя у тебя другое — подставь своё)
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE c.relkind = 'i'
          AND c.relname = 'UX_AccessRules_Subject_Resource_Permission'
          AND n.nspname = 'access'
    ) THEN
        DROP INDEX access.""UX_AccessRules_Subject_Resource_Permission"";
    END IF;
END$$;");

            // 3) Меняем типы колонок на citext
            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""SubjectType""  TYPE citext;");
            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""ResourceType"" TYPE citext;");
            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""Permission""   TYPE citext;");

            // 4) Создаём новый регистронезависимый уникальный индекс
            migrationBuilder.Sql(@"
CREATE UNIQUE INDEX IF NOT EXISTS ""UX_AccessRules_Subject_Resource_Permission""
ON access.""AccessRules""
(
  ""SubjectType"",    -- citext => регистронезависимо
  ""SubjectId"",
  ""ResourceType"",   -- citext
  ""ResourceId"",
  ""Permission""      -- citext
);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Откат: снесём CI-индекс, вернём text, создадим обычный уникальный индекс
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE c.relkind = 'i'
          AND c.relname = 'UX_AccessRules_Subject_Resource_Permission'
          AND n.nspname = 'access'
    ) THEN
        DROP INDEX access.""UX_AccessRules_Subject_Resource_Permission"";
    END IF;
END$$;");

            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""SubjectType""  TYPE text;");
            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""ResourceType"" TYPE text;");
            migrationBuilder.Sql(@"ALTER TABLE access.""AccessRules"" ALTER COLUMN ""Permission""   TYPE text;");

            migrationBuilder.Sql(@"
CREATE UNIQUE INDEX IF NOT EXISTS ""UX_AccessRules_Subject_Resource_Permission""
ON access.""AccessRules""
(
  ""SubjectType"",
  ""SubjectId"",
  ""ResourceType"",
  ""ResourceId"",
  ""Permission""
);
");
        }
    }
}
