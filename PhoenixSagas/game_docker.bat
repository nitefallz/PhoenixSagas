@echo off
SET IMAGE_NAME=phoenixsagas-gameserver
SET CONTAINER_NAME=phoenixsagas-gameserver-instance

REM Optional: Check if Docker Desktop is running
docker info >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo Docker Desktop is not running. Please start Docker Desktop and try again.
    exit /b
)

REM Stop and remove existing container if it exists
echo Checking if the container %CONTAINER_NAME% exists...
docker inspect %CONTAINER_NAME% >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    echo Stopping existing container...
    docker stop %CONTAINER_NAME%
    echo Removing existing container...
    docker rm %CONTAINER_NAME%
)

REM Build the Docker image
echo Building Docker image %IMAGE_NAME%...
docker build -t %IMAGE_NAME% .

REM Run the Docker container
echo Running new container %CONTAINER_NAME%...
docker run -d -p 8080:80 --name %CONTAINER_NAME% %IMAGE_NAME%

echo Container %CONTAINER_NAME% is now running.
pause
