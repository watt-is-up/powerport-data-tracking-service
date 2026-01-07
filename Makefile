.PHONY: up down test test-with-db rebuild

## Start MongoDB + service (dev setup)
up:
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

## Stop and remove containers
down:
	docker-compose -f docker-compose.yml -f docker-compose.override.yml down

## Rebuild images and start containers
rebuild:
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build -d

## Run unit tests (no Docker)
test:
	dotnet test ./tests/DataTrackingService.Tests

## Full integration-style test run (DB up -> tests -> DB down)
test-with-db:
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
	dotnet test ./tests/DataTrackingService.Tests
	docker-compose -f docker-compose.yml -f docker-compose.override.yml down
