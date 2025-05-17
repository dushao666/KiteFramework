# GitFetchAll.ps1
Write-Host "Starting git fetch --all..."

# Execute git fetch --all command
git fetch --all

# Check if the previous command was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "git fetch --all executed successfully." -ForegroundColor Green
} else {
    Write-Host "git fetch --all execution failed." -ForegroundColor Red
}
