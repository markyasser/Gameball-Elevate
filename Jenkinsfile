pipeline {
    agent any

    environment {
        DOTNET_ROOT = tool name: 'dotnet-sdk-7.0', type: 'DotNetCoreSdkInstaller'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        stage('Restore') {
            steps {
                script {
                    env.PATH = "${env.DOTNET_ROOT}/bin:${env.PATH}"
                }
                sh 'dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }
        stage('Test') {
            steps {
                sh 'dotnet test --no-build --verbosity normal'
            }
        }
    }

    post {
        success {
            echo 'Build succeeded!'
        }
        failure {
            echo 'Build failed!'
        }
    }
}
