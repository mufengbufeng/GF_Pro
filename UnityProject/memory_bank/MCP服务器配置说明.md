# Claude Code MCP服务器配置说明

## 概述
已为Claude Code配置了5个MCP服务器，配置文件位于 `~/.claude/mcp_servers.json`

## 配置的MCP服务器

### 1. Memory MCP Server
- **功能**: 提供持久化记忆和上下文管理功能
- **命令**: `cmd /c npx -y @modelcontextprotocol/server-memory`
- **说明**: 无需额外配置，开箱即用

### 2. Repomix Server  
- **功能**: 代码仓库分析和打包工具
- **命令**: `cmd /c npx -y repomix --mcp`
- **说明**: 用于分析和打包代码仓库，无需额外配置

### 3. Tavily MCP Server
- **功能**: 网络搜索和信息获取服务
- **命令**: `cmd /c npx -y tavily-mcp@0.1.4`
- **配置要求**: 需要设置API密钥
  ```bash
  # 设置环境变量
  export TAVILY_API_KEY="your_tavily_api_key_here"
  ```
- **获取API密钥**: 访问 [Tavily官网](https://tavily.com) 注册并获取API密钥

### 4. GitHub MCP Server
- **功能**: GitHub仓库管理和操作
- **命令**: `cmd /c npx -y @modelcontextprotocol/server-github`
- **配置要求**: 需要设置GitHub访问令牌
  ```bash
  # 设置环境变量
  export GITHUB_PERSONAL_ACCESS_TOKEN="your_github_token_here"
  ```
- **获取访问令牌**: 
  1. 登录GitHub
  2. 访问 Settings > Developer settings > Personal access tokens
  3. 生成新的token，选择所需权限

### 5. Context7 Server
- **功能**: 上下文分析和管理服务
- **命令**: `npx -y @upstash/context7-mcp`
- **说明**: 使用Upstash提供的上下文服务，可能需要Upstash账户配置

## 环境变量配置

### Windows系统
```cmd
# 临时设置（当前会话有效）
set TAVILY_API_KEY=your_tavily_api_key_here
set GITHUB_PERSONAL_ACCESS_TOKEN=your_github_token_here

# 永久设置（通过系统属性或PowerShell）
[System.Environment]::SetEnvironmentVariable("TAVILY_API_KEY", "your_key", "User")
[System.Environment]::SetEnvironmentVariable("GITHUB_PERSONAL_ACCESS_TOKEN", "your_token", "User")
```

### Linux/Mac系统
```bash
# 添加到 ~/.bashrc 或 ~/.zshrc 文件
echo 'export TAVILY_API_KEY="your_tavily_api_key_here"' >> ~/.bashrc
echo 'export GITHUB_PERSONAL_ACCESS_TOKEN="your_github_token_here"' >> ~/.bashrc
source ~/.bashrc
```

## 使用说明

1. **重启Claude Code**: 配置文件修改后需要重启Claude Code使配置生效

2. **验证配置**: 启动Claude Code后，可以通过相关命令验证MCP服务器是否正常工作

3. **故障排除**: 
   - 确保Node.js和npm已安装
   - 检查网络连接是否正常
   - 验证API密钥和访问令牌的有效性

## 注意事项

- Memory和Repomix服务器无需额外配置即可使用
- Tavily和GitHub服务器需要有效的API密钥才能正常工作
- Context7服务器可能需要Upstash账户配置
- 所有服务器都会在首次使用时自动下载和安装相关依赖

## 更新配置

如需修改配置，编辑 `~/.claude/mcp_servers.json` 文件，然后重启Claude Code。