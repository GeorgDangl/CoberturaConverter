pipeline {
	agent {
		node {
			label 'master'
            customWorkspace 'workspace/CoberturaConverter'
		}
	}
    environment {
        MyGetSource = credentials('MyGetPushSource')
        MyGetApiKey = credentials('MyGetApiKey')
        NuGetApiKey = credentials('NuGetApiKey')
        DocuApiKey = credentials('CoberturaConverter.Docu_ApiKey')
        DocuApiEndpoint = credentials('docu_api_upload_endpoint')
        GitHubAuthenticationToken = credentials('GitHubAuthenticationToken')
    }
    stages {
        stage ('Test') {
            steps {
                powershell './build.ps1 Coverage -configuration Debug'
            }
            post {
                always {
                     warnings(
                        canComputeNew: false,
                        canResolveRelativePaths: false,
                        categoriesPattern: '',
                        consoleParsers: [[parserName: 'MSBuild']],
                        defaultEncoding: '',
                        excludePattern: '',
                        healthy: '',
                        includePattern: '',
                        messagesPattern: '',
                        unHealthy: '')
                    openTasks(
                        canComputeNew: false,
                        defaultEncoding: '',
                        excludePattern: '',
                        healthy: '',
                        high: 'HACK, FIXME',
                        ignoreCase: true,
                        low: '',
                        normal: 'TODO',
                        pattern: '**/*.cs, **/*.g4, **/*.ts, **/*.js',
                        unHealthy: '')
                    xunit(
                        testTimeMargin: '3000',
                        thresholdMode: 1,
                        thresholds: [
                            failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'),
                            skipped(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
                        ],
                        tools: [
                            xUnitDotNet(deleteOutputFiles: true, failIfNotNew: true, pattern: '**/*_testresults*.xml', stopProcessingIfError: true)
                        ])
                    cobertura(
                        coberturaReportFile: 'output/cobertura_coverage.xml',
                        failUnhealthy: false,
                        failUnstable: false,
                        maxNumberOfBuilds: 0,
                        onlyStable: false,
                        zoomCoverageChart: false)
                    publishHTML([
                        allowMissing: false,
                        alwaysLinkToLastBuild: false,
                        keepAll: false,
                        reportDir: 'output/CoverageReport',
                        reportFiles: 'index.htm',
                        reportName: 'Coverage Report',
                        reportTitles: ''])
                }
            }
        }
        stage ('Deploy') {
            steps {
                powershell './build.ps1 UploadDocumentation+PublishGitHubRelease'
            }
        }
    }
    post {
        always {
            step([$class: 'Mailer',
                notifyEveryUnstableBuild: true,
                recipients: "georg@dangl.me",
                sendToIndividuals: true])
        }
    }
}
