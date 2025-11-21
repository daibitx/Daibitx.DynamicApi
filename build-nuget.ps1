# Daibitx.AspNetCore.DynamicApi NuGet 打包和发布脚本
# 按项目依赖顺序构建和打包

param(
    [Parameter(Mandatory = $false)]
    [string]$Version = "1.0.1",
    
    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory = $false)]
    [string]$NuGetApiKey = $env:NUGET_API_KEY,
    
    [Parameter(Mandatory = $false)]
    [string]$NuGetSource = "https://api.nuget.org/v3/index.json",
    
    [Parameter(Mandatory = $false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory = $false)]
    [switch]$SkipPack,
    
    [Parameter(Mandatory = $false)]
    [switch]$SkipPublish,
    
    [Parameter(Mandatory = $false)]
    [switch]$Clean
)

# 设置错误处理
$ErrorActionPreference = "Stop"

# 项目配置
$SolutionPath = Join-Path $PSScriptRoot "src\Daibitx.AspNetCore.DynamicApi.sln"
$OutputPath = Join-Path $PSScriptRoot "artifacts"
$PackagesPath = Join-Path $OutputPath "packages"

# 项目列表（按依赖顺序）
$Projects = @(
    @{
        Name = "Daibitx.AspNetCore.DynamicApi.Abstraction"
        Path = "src\Daibitx.AspNetCore.DynamicApi.Abstraction\Daibitx.AspNetCore.DynamicApi.Abstraction.csproj"
        PackageId = "Daibitx.AspNetCore.DynamicApi.Abstraction"
    },
    @{
        Name = "Daibitx.AspNetCore.DynamicApi.Runtime"
        Path = "src\Daibitx.AspNetCore.DynamicApi.Runtime\Daibitx.AspNetCore.DynamicApi.Runtime.csproj"
        PackageId = "Daibitx.AspNetCore.DynamicApi.Runtime"
    },
    @{
        Name = "Daibitx.AspNetCore.DynamicApi"
        Path = "src\Daibitx.AspNetCore.DynamicApi\Daibitx.AspNetCore.DynamicApi.csproj"
        PackageId = "Daibitx.AspNetCore.DynamicApi"
    }
)

# 颜色输出函数
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "[✓] $Message" "Green"
}

function Write-Info {
    param([string]$Message)
    Write-ColorOutput "[i] $Message" "Cyan"
}

function Write-Warning {
    param([string]$Message)
    Write-ColorOutput "[!] $Message" "Yellow"
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "[✗] $Message" "Red"
}

# 检查 .NET SDK
function Test-DotNetSDK {
    try {
        $dotnetVersion = dotnet --version
        Write-Info ".NET SDK 版本: $dotnetVersion"
        return $true
    }
    catch {
        Write-Error ".NET SDK 未安装或不在 PATH 中"
        return $false
    }
}

# 清理输出目录
function Clear-Artifacts {
    Write-Info "清理输出目录..."
    if (Test-Path $OutputPath) {
        Remove-Item -Path $OutputPath -Recurse -Force
        Write-Success "输出目录已清理"
    }
    New-Item -ItemType Directory -Path $PackagesPath -Force | Out-Null
}

# 还原解决方案
function Restore-Solution {
    Write-Info "还原 NuGet 包..."
    dotnet restore $SolutionPath
    if ($LASTEXITCODE -ne 0) {
        throw "NuGet 还原失败"
    }
    Write-Success "NuGet 包还原完成"
}

# 构建项目
function Build-Project {
    param(
        [string]$ProjectPath,
        [string]$ProjectName
    )
    
    Write-Info "构建项目: $ProjectName"
    $fullProjectPath = Join-Path $PSScriptRoot $ProjectPath
    
    dotnet build $fullProjectPath `
        --configuration $Configuration `
        --no-restore `
        -p:Version=$Version `
        -p:AssemblyVersion=$Version `
        -p:FileVersion=$Version
    
    if ($LASTEXITCODE -ne 0) {
        throw "项目构建失败: $ProjectName"
    }
    Write-Success "项目构建完成: $ProjectName"
}

# 打包项目
function Pack-Project {
    param(
        [string]$ProjectPath,
        [string]$ProjectName,
        [string]$PackageId
    )
    
    Write-Info "打包项目: $ProjectName"
    $fullProjectPath = Join-Path $PSScriptRoot $ProjectPath
    
    dotnet pack $fullProjectPath `
        --configuration $Configuration `
        --no-build `
        --output $PackagesPath `
        -p:PackageVersion=$Version `
        -p:Version=$Version
    
    if ($LASTEXITCODE -ne 0) {
        throw "项目打包失败: $ProjectName"
    }
    
    # 检查生成的包
    $packageFile = Join-Path $PackagesPath "$PackageId.$Version.nupkg"
    if (Test-Path $packageFile) {
        Write-Success "包已生成: $PackageId.$Version.nupkg"
    } else {
        Write-Warning "未找到预期的包文件: $packageFile"
    }
}

# 发布包到 NuGet
function Publish-Package {
    param(
        [string]$PackageId,
        [string]$Version
    )
    
    if ([string]::IsNullOrWhiteSpace($NuGetApiKey)) {
        Write-Warning "未提供 NuGet API Key，跳过发布"
        return
    }
    
    $packageFile = Join-Path $PackagesPath "$PackageId.$Version.nupkg"
    
    if (-not (Test-Path $packageFile)) {
        Write-Error "包文件不存在: $packageFile"
        return
    }
    
    Write-Info "发布包到 NuGet: $PackageId.$Version"
    
    try {
        dotnet nuget push $packageFile `
            --api-key $NuGetApiKey `
            --source $NuGetSource `
            --skip-duplicate
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "包发布成功: $PackageId.$Version"
        } else {
            Write-Error "包发布失败: $PackageId.$Version"
        }
    }
    catch {
        Write-Error "发布过程中发生错误: $_"
    }
}

# 主执行流程
try {
    Write-ColorOutput "========================================" "Cyan"
    Write-ColorOutput "Daibitx.AspNetCore.DynamicApi 打包脚本" "Cyan"
    Write-ColorOutput "版本: $Version" "Cyan"
    Write-ColorOutput "配置: $Configuration" "Cyan"
    Write-ColorOutput "========================================" "Cyan"
    Write-Host
    
    # 检查 .NET SDK
    if (-not (Test-DotNetSDK)) {
        exit 1
    }
    
    # 清理（如果需要）
    if ($Clean) {
        Clear-Artifacts
    } else {
        New-Item -ItemType Directory -Path $PackagesPath -Force | Out-Null
    }
    
    # 还原包
    if (-not $SkipBuild) {
        Restore-Solution
    }
    
    # 处理每个项目
    foreach ($project in $Projects) {
        Write-ColorOutput "----------------------------------------" "Gray"
        Write-ColorOutput "处理项目: $($project.Name)" "Yellow"
        Write-ColorOutput "----------------------------------------" "Gray"
        
        try {
            # 构建
            if (-not $SkipBuild) {
                Build-Project -ProjectPath $project.Path -ProjectName $project.Name
            }
            
            # 打包
            if (-not $SkipPack) {
                Pack-Project -ProjectPath $project.Path -ProjectName $project.Name -PackageId $project.PackageId
            }
            
            # 发布
            if (-not $SkipPublish) {
                Publish-Package -PackageId $project.PackageId -Version $Version
            }
            
            Write-Success "项目处理完成: $($project.Name)"
        }
        catch {
            Write-Error "项目处理失败: $($project.Name) - $_"
            throw
        }
        
        Write-Host
    }
    
    # 总结
    Write-ColorOutput "========================================" "Cyan"
    Write-ColorOutput "打包完成！" "Green"
    Write-ColorOutput "输出目录: $PackagesPath" "Cyan"
    Write-ColorOutput "========================================" "Cyan"
    
    # 列出生成的包
    if (Test-Path $PackagesPath) {
        Write-Info "生成的包:"
        Get-ChildItem -Path $PackagesPath -Filter "*.nupkg" | ForEach-Object {
            Write-ColorOutput "  - $($_.Name)" "Green"
        }
    }
}
catch {
    Write-Error "脚本执行失败: $_"
    exit 1
}