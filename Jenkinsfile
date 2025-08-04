pipeline {
    agent any

    stages {
        stage('Check .NET SDK') {
            steps {
                bat 'dotnet --version'
                bat 'dotnet --list-sdks'
            }
        }

        stage('Clone') {
            steps {
                echo 'Cloning source code'
                git branch: 'main', url: 'https://github.com/Qthanh074/GIT-TKPM.git'
            }
        }

        stage('Restore package') {
            steps {
                echo 'Restoring packages...'
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building project...'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Tests') {
            steps {
                echo 'Running tests...'
                bat 'dotnet test --no-build --verbosity normal'
            }
        }

        stage('Publish to ./publish folder') {
            steps {
                echo 'Publishing to ./publish folder...'
                bat 'dotnet publish -c Release -o "%WORKSPACE%\\publish"'
            }
        }

        stage('Copy to IIS folder') {
            steps {
                echo 'Copying to IIS root folder...'
                bat 'iisreset /stop'
                bat '''
                if exist "C:\\inetpub\\wwwroot\\TrienKhaiPM" (
                    rmdir /S /Q "C:\\inetpub\\wwwroot\\TrienKhaiPM"
                )
                mkdir "C:\\inetpub\\wwwroot\\TrienKhaiPM"
                xcopy "%WORKSPACE%\\publish" "C:\\inetpub\\wwwroot\\TrienKhaiPM" /E /Y /I /R
                '''
                bat 'iisreset /start'
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo 'Creating IIS Website if not exists...'
                powershell '''
                    Import-Module WebAdministration
                    if (-not (Test-Path IIS:\\Sites\\TKPMJ)) {
                        New-Website -Name "TKPMJ" -Port 83 -PhysicalPath "C:\\inetpub\\wwwroot\\TrienKhaiPM" -Force
                    }
                '''
            }
        }
    }
}
