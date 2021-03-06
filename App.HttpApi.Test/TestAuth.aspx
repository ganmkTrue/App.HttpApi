﻿<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="App.Components" %>
<%@ Import Namespace="App.HttpApi" %>
<%@ Import Namespace="System.Security.Principal" %>
<%@ Import Namespace="App" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            ShowUser();
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        IPrincipal p = AuthHelper.Login("Surfsky", null, DateTime.Now.AddDays(1));
        ShowUser(p);
    }
    protected void btnLogin2_Click(object sender, EventArgs e)
    {
        IPrincipal p = AuthHelper.Login("Kevin", new string[] { "Admins" }, DateTime.Now.AddDays(1));
        ShowUser(p);
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        AuthHelper.Logout();
        ShowUser(null);
    }

    void ShowUser()
    {
        ShowUser(HttpContext.Current.User);
    }
    void ShowUser(IPrincipal p)
    {
        if (p == null || p.Identity.Name == "")
            this.lblInfo.Text = "未登录";
        else
        {
            this.lblInfo.Text = p.Identity.Name + "    ";
            if (p.IsInRole("Admins"))
                lblInfo.Text += "Admins";
        }
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="HttpApi.App.DemoClass.axd/js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="lblInfo"  Text="当前用户信息" />
        <ul>
            <li><a id="btn4" onclick="App.DemoClass.Login(function (data) { }, 'btn4')" style="text-decoration:underline">登录</a></li>
            <li><a id="btn5" onclick="App.DemoClass.Logout(function (data) { }, 'btn5')" style="text-decoration:underline">注销</a></li>
            <li>&nbsp;</li>
            <li><a id="btn1" onclick="App.DemoClass.LimitLogin(function (data) { }, 'btn1')" style="text-decoration:underline">登录后才可以调用</a></li>
            <li><a id="btn2" onclick="App.DemoClass.LimitUser(function (data) { }, 'btn2');" style="text-decoration:underline">指定用户（Kevin）才可以调用</a></li>
            <li><a id="btn3" onclick="App.DemoClass.LimitRole(function (data) { }, 'btn3');" style="text-decoration:underline">指定角色（Admins）才可以调用</a></li>
        </ul>
        <br />
        <br />
        <asp:Button runat="server" ID="btnLogin" Text="登录为surfsky" OnClick="btnLogin_Click" />
        <asp:Button runat="server" ID="btnLogin2" Text="登录为kevin" OnClick="btnLogin2_Click" />
        &nbsp;<asp:Button runat="server" ID="btnLogout" Text="注销" OnClick="btnLogout_Click" />
    </div>

    <pre>
【使用方法】
（1）在需认证的HttpApi方法上加上特性标签
        - AllowAnonymous：是否允许匿名访问（默认为true）
        - AllowUsers ：允许访问的用户（用逗号分隔）
        - AuthRoles ：允许访问的角色（用逗号分隔）
    示例
        [HttpApi()]
        public string Login()
        {
            AuthHelper.Login("Admin", null, DateTime.Now.AddDays(1));
            return "登录成功";
        }
        [HttpApi(AuthLogin=true)]
        public string LimitLogin()
        {
            return "用户必须登录后才能访问该接口";
        }
        [HttpApi(AuthUsers = "Admin,Kevin")]
        public string LimitUser()
        {
            return "指定用户才能访问该方法（Admin,Kevin）";
        }
        [HttpApi(AuthRoles = "Admins")]
        public string LimitRole()
        {
            return "指定角色才能访问该方法（Admins）";
        }

（2）在Global.asax.cs中写下以下代码，从cookie验票中获取当前用户信息
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthHelper.LoadCookieTicket();
        }

（4）可在Web.confit中配置错误时返回格式（不设置的话默认为DataResult）
    // 若为HttpError，会输出标准的HTTP错误，浏览器的话会跳转到对应的错误页面
    // 若为DataResult，直接输出DataResult错误信息


【关于HttpContext.Current.User】
HttpContext.Current.User 保存了当前访问用户的信息
    - 含两个基本接口
        IPrincipal p = HttpContext.Current.User;
        String name = p.Name;
        bool b = p.IsInRole("Admins");

    </pre>
    </form>
</body>
</html>
