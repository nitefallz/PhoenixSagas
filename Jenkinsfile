pipeline {
    agent any

    environment {
        // Define any environment variables needed for the build
        DOTNET_CORE_SDK_VERSION = '7.0' // Adjust this to match the SDK version you're using
    }

    stages {
        stage('Checkout') {
            steps {
                // Checkout the code from the GitHub repository
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }

        stage('Restore') {
            steps {
                // Restore dependencies specified in the project file
                sh 'dotnet restore PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj'
            }
        }

        stage('Build') {
            steps {
                // Build the project
                sh 'dotnet build PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
            }
        }

        stage('Test') {
            steps {
                // Run tests - adjust the path to where your test projects are located
                echo 'No tests specified. Add commands to run tests here.'
                // Example: sh 'dotnet test YourTestProject/YourTestProject.csproj'
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying application...'
                // Add your deployment commands here
                // This could involve copying the build output to a server or running deployment scripts
            }
        }
    }

    post {
        // Define actions to take after the pipeline runs
        success {
            echo 'Build and test stages completed successfully.'
        }
        failure {
            echo 'An error occurred during the build.'
        }
    }
}