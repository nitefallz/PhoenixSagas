pipeline {
    agent any

    environment {
        // Using .NET 7.0 SDK
        DOTNET_CORE_SDK_VERSION = '7.0'
        DOCKER_IMAGE = 'nitefallz/mystuff' // Docker Hub username/repo
        DOCKER_TAG = 'latest'
        DOCKER_CREDENTIALS_ID = 'docker-hub-credentials' // ID for Docker Hub credentials stored in Jenkins
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'No tests specified. Add commands to run tests here.'
                // Example: sh 'dotnet test YourTestProject/YourTestProject.csproj'
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    sh 'docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} .'
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
                    echo 'Deploying application...'
                    // Implement deployment logic here based on your target environment
                    // This might involve SSH commands to a server, Kubernetes deployment commands, etc.
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
