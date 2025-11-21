# Daibitx.AspNetCore.DynamicApi NuGet 打包和发布指南

## 项目依赖关系

本项目包含三个 NuGet 包，按以下依赖顺序构建：

1. **Daibitx.AspNetCore.DynamicApi.Abstraction** - 基础抽象层
   - 无依赖
   - 包含接口和属性定义

2. **Daibitx.AspNetCore.DynamicApi.Runtime** - 源生成器
   - 依赖 Abstraction
   - Roslyn 源生成器实现

3. **Daibitx.AspNetCore.DynamicApi** - 主包
   - 依赖 Abstraction 和 Runtime
   - 最终用户使用的包

## 快速开始

### 1. 仅打包（本地测试）

```powershell
# 打包所有项目到本地 artifacts/packages 目录
.\build-nuget.ps1 -Version "1.0.0"
```

### 2. 打包并发布到 NuGet

```powershell
# 需要设置 NuGet API Key
.\build-nuget.ps1 -Version "1.0.0" -NuGetApiKey "your-api-key-here"
```

### 3. 完整构建流程

```powershell
# 清理、构建、打包、发布
.\build-nuget.ps1 -Version "1.0.1" -Clean -NuGetApiKey "your-api-key-here"
```

## 参数说明

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Version` | string | "1.0.0" | 版本号（会应用到所有包） |
| `Configuration` | string | "Release" | 构建配置（Debug/Release） |
| `NuGetApiKey` | string | "" | NuGet API Key（为空则跳过发布） |
| `NuGetSource` | string | "https://api.nuget.org/v3/index.json" | NuGet 源地址 |
| `SkipBuild` | switch | `$false` | 跳过构建步骤 |
| `SkipPack` | switch | `$false` | 跳过多包步骤 |
| `SkipPublish` | switch | `$false` | 跳过发布步骤 |
| `Clean` | switch | `$false` | 清理输出目录 |

## 使用示例

### 场景 1：本地开发测试

```powershell
# 只打包，不发布
.\build-nuget.ps1 -Version "1.0.0-beta1" -SkipPublish

# 输出在 artifacts/packages 目录
# 可以在本地测试这些包
```

### 场景 2：发布预览版本

```powershell
# 发布预览版到 NuGet
.\build-nuget.ps1 -Version "1.0.0-preview1" -NuGetApiKey "your-key"
```

### 场景 3：发布正式版本

```powershell
# 完整流程：清理、构建、打包、发布
.\build-nuget.ps1 -Version "1.0.0" -Clean -NuGetApiKey "your-key"
```

### 场景 4：仅重新打包

```powershell
# 如果已经构建过，只重新打包
.\build-nuget.ps1 -Version "1.0.0" -SkipBuild -SkipPublish
```

## 输出结构

```
artifacts/
└── packages/
    ├── Daibitx.AspNetCore.DynamicApi.Abstraction.1.0.0.nupkg
    ├── Daibitx.AspNetCore.DynamicApi.Runtime.1.0.0.nupkg
    └── Daibitx.AspNetCore.DynamicApi.1.0.0.nupkg
```

## 获取 NuGet API Key

1. 登录 [NuGet.org](https://www.nuget.org/)
2. 点击右上角用户名 → **API Keys**
3. 点击 **Create** 创建新 Key
4. 配置 Key 名称和权限
5. 复制生成的 Key（只显示一次）

## 版本号规范

建议使用 [Semantic Versioning](https://semver.org/)：

- `1.0.0` - 正式版本
- `1.0.0-beta1` - 测试版本
- `1.0.0-preview1` - 预览版本
- `1.0.0-rc1` - 候选版本

## 故障排除

### 问题 1：.NET SDK 未找到

**错误信息**：`.NET SDK 未安装或不在 PATH 中`

**解决方案**：
- 安装 [.NET SDK](https://dotnet.microsoft.com/download)
- 确保 `dotnet` 命令在 PATH 中

### 问题 2：NuGet 发布失败

**错误信息**：`Response status code does not indicate success: 403 (Forbidden)`

**解决方案**：
- 检查 API Key 是否正确
- 确认 Key 有发布权限
- 检查包 ID 是否已被占用

### 问题 3：构建失败

**错误信息**：`项目构建失败`

**解决方案**：
- 检查项目依赖是否正确
- 确保所有项目都能独立构建
- 查看详细的错误信息

## 自动化建议

### GitHub Actions 集成

```yaml
name: Build and Publish NuGet

on:
  push:
    tags:
      - 'v*'

jobs:
  build-and-publish:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Build and Publish
        run: |
          $Version = $env:GITHUB_REF -replace 'refs/tags/v', ''
          .\build-nuget.ps1 -Version $Version -NuGetApiKey ${{ secrets.NUGET_API_KEY }}
```

### Azure DevOps 集成

```yaml
trigger:
  tags:
    include:
      - v*

pool:
  vmImage: 'windows-latest'

steps:
- task: PowerShell@2
  inputs:
    filePath: 'build-nuget.ps1'
    arguments: '-Version $(Build.SourceBranchName) -NuGetApiKey $(NuGetApiKey)'
```

## 安全建议

1. **不要在代码中硬编码 API Key**
2. **使用环境变量或 CI/CD 密钥管理**
3. **定期轮换 API Key**
4. **为不同环境使用不同的 Key**

## 脚本功能特点

- ✅ 按依赖顺序自动处理项目
- ✅ 彩色输出，易于阅读
- ✅ 详细的错误处理和日志
- ✅ 支持增量构建
- ✅ 自动跳过重复发布
- ✅ 完整的参数控制
- ✅ 输出目录结构清晰