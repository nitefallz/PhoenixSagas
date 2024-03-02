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

        stage('Restore') {
            steps {
                sh 'dotnet restore PhoenixSagas/PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build PhoenixSagas/PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release'
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
                    sh 'dotnet pack PhoenixSagas/PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release -o ./nupkgs'
                    // Push the package to your NuGet server
                    sh 'dotnet nuget push "./nupkgs/*.nupkg" --source ${NUGET_SERVER_URL} --api-key ${NUGET_API_KEY}'
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    dir('PhoenixSagas/') { 
                        sh 'docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} .'
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
                    sh """
                    docker stop phoenixsagas-tcpserver-container || true
                    docker rm phoenixsagas-tcpserver-container || true
                    """
                    sh """
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
