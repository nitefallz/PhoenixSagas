pipeline {
    agent any

    environment {
        // Using .NET 7.0 SDK
        DOTNET_CORE_SDK_VERSION = '7.0'
        DOCKER_IMAGE = 'nitefallz/mystuff' // Adjust with your Docker Hub repo name
        DOCKER_TAG = 'latest'
        DOCKER_CREDENTIALS_ID = 'docker-hub-credentials' // Use the actual ID for Docker Hub credentials
        NUGET_SERVER_URL = 'https://gomezdev.hopto.org:8090/NugetServer/nuget' // Your NuGet server URL
        NUGET_API_KEY = '711cf1f2-d71d-4a75-9293-ecc6e2992228' // Your NuGet server API Key
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }

        
        stage('Restore NuGet Packages') {
            steps {
                // Use the newly added source along with the default nuget.org source
                sh 'dotnet restore PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/Models/PhoenixSagas.Models.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/TCPServer/PhoenixSagas.TCPServer.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
            }
        }

        stage('Build Projects') {
            steps {
                sh 'dotnet build PhoenixSagas/Models/PhoenixSagas.Models.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
            }
        }


        stage('Test') {
            steps {
                echo 'No tests specified. Add commands to run tests here.'
                // Ideally, include steps to run your project's tests
            }
        }

        stage('Package and Push NuGet') {
            steps {
                script {
                    // Package the .NET project
                    sh 'dotnet pack PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release -o ./nupkgs'
                    sh 'dotnet pack PhoenixSagas/Models/PhoenixSagas.Models.csproj --configuration Release -o ./nupkgs'
                    
                    // Find and push each package to the NuGet server
                    def nugetPackages = findFiles(glob: 'nupkgs/*.nupkg')
                    nugetPackages.each {
                        sh "dotnet nuget push '${it.path}' --source ${NUGET_SERVER_URL} --api-key ${NUGET_API_KEY} --skip-duplicate"
                    }
                }
            }
        }

        
        stage('Build Docker Image') {
            steps {
                script {
                    dir('PhoenixSagas/') { 
                        // Add --build-arg to pass the NUGET_SERVER_URL to the Docker build
                        sh 'docker build --build-arg NUGET_SOURCE=${NUGET_SERVER_URL} -t ${DOCKER_IMAGE}:${DOCKER_TAG} .'
                    }
                }
            }
        }

        stage('Push Docker Image') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: "${DOCKER_CREDENTIALS_ID}", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                        sh 'echo $DOCKER_PASSWORD | docker login --username $DOCKER_USERNAME --password-stdin'
                        sh 'docker push ${DOCKER_IMAGE}:${DOCKER_TAG}'
                    }
                }
            }
        }

        stage('Deploy') {
            steps {
                script {
                    // Commands to stop, remove, and run a new container instance
                    sh """
                    docker stop phoenixsagas-tcpserver-container || true
                    docker rm phoenixsagas-tcpserver-container || true
                    docker run -d --name phoenixsagas-tcpserver-container -p 4000:4000 ${DOCKER_IMAGE}:${DOCKER_TAG}
                    """
                }
            }
        }
    }

    post {
        success {
            echo 'Build, Docker image push, and deployment stages completed successfully.'
        }
        failure {
            echo 'An error occurred during the build.'
        }
    }
}
