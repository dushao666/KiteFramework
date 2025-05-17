# GitFetchAll.ps1
Write-Host "开始执行 git fetch --all..."

# 执行 git fetch --all 命令
git fetch --all

# 检查上一个命令是否成功
if ($LASTEXITCODE -eq 0) {
    Write-Host "git fetch --all 执行成功。" -ForegroundColor Green
} else {
    Write-Host "git fetch --all 执行失败。" -ForegroundColor Red
}
