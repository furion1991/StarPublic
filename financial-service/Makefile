ENV ?= dev
CONTROLLER ?= NewController
MIGRATION ?= Initial
include .env.${ENV}-example
-include .env

controller:
	dotnet aspnet-codegenerator -p . controller -name ${CONTROLLER} -outDir ./Controllers

up:
ifeq (,$(wildcard ./.env))
	cp .env.${ENV}-example .env
endif
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml up -d --build

down:
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml down

restart: 
	make down
	make up

db:
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml exec $(DATABASE_HOST) psql -U$(DATABASE_USER) -d$(DATABASE_NAME)

migration-add:
	make dotnet-ef-init
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml exec asp_financial dotnet dotnet-ef migrations add ${MIGRATION}

migrate:
	make dotnet-ef-init
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml exec asp_financial dotnet dotnet-ef database update

dotnet-ef-init:
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml exec asp_financial dotnet new tool-manifest --force
	cd docker && docker-compose --env-file ../.env -f docker-compose.${ENV_MODE}.yml exec asp_financial dotnet tool install --local dotnet-ef --version 8.0.8
