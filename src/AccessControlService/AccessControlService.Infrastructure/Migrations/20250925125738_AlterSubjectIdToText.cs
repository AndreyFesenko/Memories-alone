using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControlService.Infrastructure.Migrations
{
    public partial class AlterSubjectIdToText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // На всякий случай убеждаемся, что схема есть
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS access;");

            // Меняем тип SubjectId с uuid -> text.
            // Postgres может сконвертировать индекс/уникальные ключи автоматически.
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (
    SELECT 1
    FROM information_schema.columns
    WHERE table_schema='access' AND table_name='AccessRules' AND column_name='SubjectId'
      AND udt_name='uuid'
  ) THEN
    ALTER TABLE access.""AccessRules""
      ALTER COLUMN ""SubjectId"" TYPE text
      USING ""SubjectId""::text;
  END IF;
END $$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Обратное преобразование text -> uuid (если все значения — валидные GUID)
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (
    SELECT 1
    FROM information_schema.columns
    WHERE table_schema='access' AND table_name='AccessRules' AND column_name='SubjectId'
      AND data_type='text'
  ) THEN
    ALTER TABLE access.""AccessRules""
      ALTER COLUMN ""SubjectId"" TYPE uuid
      USING NULLIF(""SubjectId"", '')::uuid;
  END IF;
END $$;");
        }
    }
}
