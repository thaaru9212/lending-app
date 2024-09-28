pipeline {
    agent any
    
    environment {
        EMAIL_RECIPIENT = 'thaaru1220@gmail.com'
        STAGING_ENV = 'staging-environment'
        PRODUCTION_ENV = 'production-environment'
        AWS_ECR_REPO = '600627318987.dkr.ecr.ap-southeast-2.amazonaws.com/jenkins'
        AWS_REGION = 'ap-southeast-2'
        APP_NAME = 'lending-app'
        DOTNET_VERSION = '6.0'
    }
    
    stages {
        // Stage 1: Build the Docker image with the runtime
        stage('Build') {
            steps {
                script {
                    echo 'Building the .NET project using Docker...'
                    
                    // Build the application with the Dockerfile
                    sh 'docker build -t lending-app-build:latest -f Dockerfile.runtime .'
                }
            }
        }
        
        // Stage 2: Run Unit and Integration Tests inside the container
        stage('Unit and Integration Tests') {
            steps {
                script {
                    echo 'Running unit and integration tests inside Docker container...'
                    sh 'docker run --rm lending-app-build:latest dotnet test'
                }
            }
            post {
                always {
                    emailext subject: "Test Stage Status: ${currentBuild.result}",
                             body: "The test stage has completed with status: ${currentBuild.result}",
                             attachLog: true,
                             to: "${EMAIL_RECIPIENT}"
                }
            }
        }

        // Stage 3: Code Quality Analysis
        stage('Code Quality Analysis') {
            steps {
                script {
                    echo 'Running code quality analysis with SonarQube...'
                    withSonarQubeEnv('SonarQube') {
                        sh 'sonar-scanner -Dsonar.projectKey=lending-app -Dsonar.sources=src'
                    }
                }
            }
        }
        
        // Stage 4: Security Scan
        stage('Security Scan') {
            steps {
                script {
                    echo 'Performing security scan using OWASP ZAP...'
                    sh 'zap-cli quick-scan http://localhost:8080'
                }
            }
            post {
                always {
                    emailext subject: "Security Scan Stage Status: ${currentBuild.result}",
                             body: "The security scan stage has completed with status: ${currentBuild.result}",
                             attachLog: true,
                             to: "${EMAIL_RECIPIENT}"
                }
            }
        }
        
        // Stage 5: Deploy to Staging Environment
        stage('Deploy to Staging') {
            steps {
                script {
                    echo 'Deploying Docker image to staging environment...'
                    sh 'aws ecr get-login-password --region ${AWS_REGION} | docker login --username AWS --password-stdin ${AWS_ECR_REPO}'
                    
                    // Tag and Push Docker image to ECR
                    sh 'docker tag lending-app-build:latest ${AWS_ECR_REPO}:latest'
                    sh 'docker push ${AWS_ECR_REPO}:latest'
                    
                    // AWS CodeDeploy deployment to staging
                    sh '''
                    aws deploy create-deployment --application-name ${APP_NAME} \
                        --deployment-config-name CodeDeployDefault.AllAtOnce \
                        --deployment-group-name ${STAGING_ENV} \
                        --github-location repository=thaaru9212/lending-app,commitId=${GIT_COMMIT}
                    '''
                }
            }
        }
        
        // Stage 6: Integration Tests on Staging
        stage('Integration Tests on Staging') {
            steps {
                script {
                    echo 'Running integration tests on staging...'
                    sh 'docker run --rm ${AWS_ECR_REPO}:latest dotnet test'
                }
            }
        }
        
        // Stage 7: Deploy to Production
        stage('Deploy to Production') {
            when {
                expression { return input(message: 'Approve for production deployment?') == 'yes' }
            }
            steps {
                script {
                    echo 'Deploying to production environment...'
                    sh '''
                    aws deploy create-deployment --application-name ${APP_NAME} \
                        --deployment-config-name CodeDeployDefault.AllAtOnce \
                        --deployment-group-name ${PRODUCTION_ENV} \
                        --github-location repository=thaaru9212/lending-app,commitId=${GIT_COMMIT}
                    '''
                }
            }
        }
        
        // Stage 8: Monitoring and Alerting
        stage('Monitoring and Alerting') {
            steps {
                script {
                    echo 'Setting up monitoring using New Relic...'
                    sh 'newrelic monitor-app --app ${APP_NAME}'
                }
            }
        }
    }
    
    post {
        always {
            echo 'Pipeline execution completed!'
        }

        success {
            emailext subject: "Pipeline Success: ${env.JOB_NAME} - ${env.BUILD_NUMBER}",
                     body: """<p>The pipeline has completed successfully.</p>
                              <p>Check console output at <a href='${env.BUILD_URL}'>${env.JOB_NAME} [${env.BUILD_NUMBER}]</a></p>""",
                     to: "${EMAIL_RECIPIENT}"
        }

        failure {
            emailext subject: "Pipeline Failure: ${env.JOB_NAME} - ${env.BUILD_NUMBER}",
                     body: """<p>The pipeline has failed.</p>
                              <p>Check console output at <a href='${env.BUILD_URL}'>${env.JOB_NAME} [${env.BUILD_NUMBER}]</a></p>""",
                     to: "${EMAIL_RECIPIENT}"
        }

        unstable {
            emailext subject: "Pipeline Unstable: ${env.JOB_NAME} - ${env.BUILD_NUMBER}",
                     body: """<p>The pipeline completed with some warnings or unstable results.</p>
                              <p>Check console output at <a href='${env.BUILD_URL}'>${env.JOB_NAME} [${env.BUILD_NUMBER}]</a></p>""",
                     to: "${EMAIL_RECIPIENT}"
        }
    }
}
