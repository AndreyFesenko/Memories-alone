using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControlService.Infrastructure.Migrations
{
    public partial class FixAccessRulesAllColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 0) схема
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS access;");

            // 1) если таблица в public — перенесём в access
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='AccessRules')
     AND NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='access' AND table_name='AccessRules') THEN
    ALTER TABLE public.""AccessRules"" SET SCHEMA access;
  END IF;
END $$;");

            // 2) переименования возможных старых имён колонок -> ожидаемые EF имена
            migrationBuilder.Sql(@"
DO $$
BEGIN
  -- SubjectType
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='subjecttype') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN subjecttype TO ""SubjectType"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='subject_type') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN subject_type TO ""SubjectType"";
  END IF;

  -- SubjectId
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='subjectid') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN subjectid TO ""SubjectId"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='subject_id') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN subject_id TO ""SubjectId"";
  END IF;

  -- ResourceType
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='resourcetype') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN resourcetype TO ""ResourceType"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='resource_type') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN resource_type TO ""ResourceType"";
  END IF;

  -- ResourceId
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='resourceid') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN resourceid TO ""ResourceId"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='resource_id') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN resource_id TO ""ResourceId"";
  END IF;

  -- Permission
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='permission') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN permission TO ""Permission"";
  END IF;

  -- ExpiresAt
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='expiresat') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN expiresat TO ""ExpiresAt"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='expires_at') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN expires_at TO ""ExpiresAt"";
  END IF;

  -- GrantedBy
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='grantedby') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN grantedby TO ""GrantedBy"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='granted_by') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN granted_by TO ""GrantedBy"";
  END IF;

  -- CreatedAt
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='createdat') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN createdat TO ""CreatedAt"";
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='created_at') THEN
    ALTER TABLE access.""AccessRules"" RENAME COLUMN created_at TO ""CreatedAt"";
  END IF;
END $$;");

            // 3) добавление недостающих колонок
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='SubjectType') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""SubjectType"" text NOT NULL DEFAULT 'User';
    ALTER TABLE access.""AccessRules"" ALTER COLUMN ""SubjectType"" DROP DEFAULT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='SubjectId') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""SubjectId"" text NOT NULL DEFAULT '';
    ALTER TABLE access.""AccessRules"" ALTER COLUMN ""SubjectId"" DROP DEFAULT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ResourceType') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""ResourceType"" text NOT NULL DEFAULT 'Memory';
    ALTER TABLE access.""AccessRules"" ALTER COLUMN ""ResourceType"" DROP DEFAULT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ResourceId') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""ResourceId"" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    ALTER TABLE access.""AccessRules"" ALTER COLUMN ""ResourceId"" DROP DEFAULT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='Permission') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""Permission"" text NOT NULL DEFAULT 'View';
    ALTER TABLE access.""AccessRules"" ALTER COLUMN ""Permission"" DROP DEFAULT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='ExpiresAt') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""ExpiresAt"" timestamp with time zone NULL;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='GrantedBy') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""GrantedBy"" uuid NULL;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='access' AND table_name='AccessRules' AND column_name='CreatedAt') THEN
    ALTER TABLE access.""AccessRules"" ADD COLUMN ""CreatedAt"" timestamp with time zone NOT NULL DEFAULT now();
  END IF;
END $$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ничего не откатываем — фиксер точечно правит прод-таблицу.
        }
    }
}
