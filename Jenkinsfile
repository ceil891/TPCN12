pipeline {
    agent any

    stages {
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
                bat 'dotnet publish -c Release -o ./publish'
            }
        }
        stage('Check .NET SDK') {
    steps {
        bat 'dotnet --version'
        bat 'dotnet --list-sdks'
    }
}

       stage('Copy to IIS folder') {
    steps {
        echo 'Copying to IIS root folder...'
        bat 'iisreset /stop'
        bat '''
        if exist "%WORKSPACE%\\publish" (
            xcopy "%WORKSPACE%\\publish" "C:\\inetpub\\wwwroot\\TrienKhaiPhamMem4" /E /Y /I /R
        ) else (
            echo "Publish folder not found!"
            exit /b 1
        )
        '''
        bat 'iisreset /start'
    }
}


        stage('Deploy to IIS') {
            steps {
                echo 'Creating IIS Website if not exists...'
                powershell '''
                    Import-Module WebAdministration
                    if (-not (Test-Path IIS:\\Sites\\TKPM4)) {
                        New-Website -Name "TKPM4" -Port 83 -PhysicalPath "C:\\inetpub\\wwwroot\\TrienKhaiPhamMem4" -Force
                    }
                '''
            }
        }
    }
}
