pipeline {
    agent any

    environment {
        // Using .NET 7.0 SDK
        DOTNET_CORE_SDK_VERSION = '7.0'
        DOCKER_IMAGE = 'nitefallz/mystuff' // Adjust with your Docker Hub repo name
        DOCKER_TAG = 'latest'
        DOCKER_CREDENTIALS_ID = 'docker-hub-credentials' // Use the actual ID for Docker Hub credentials
        NUGET_SERVER_URL = 'http://gomezdev.hopto.org:8090/NugetServer/nuget' // Your NuGet server URL
        NUGET_API_KEY = '711cf1f2-d71d-4a75-9293-ecc6e2992228' // Your NuGet server API Key
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }

        stage('Add NuGet Source') {
            steps {
                script {
                    // Add the custom NuGet source defined in environment variables
                    sh 'dotnet nuget add source "${NUGET_SERVER_URL}" --name CustomNuGetSource'
                }
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                // Use the newly added source along with the default nuget.org source
                sh 'dotnet restore PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
            }
        }

        stage('Build Projects') {
            steps {
                sh 'dotnet build PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
            }
        }

        // The rest of your pipeline stages remain unchanged...
    }

    // Post-build actions...
}
