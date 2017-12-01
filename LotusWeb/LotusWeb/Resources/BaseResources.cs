using LotusWeb.Data.Contexts;
using LotusWeb.Logic.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Resources
{
    public class BaseResources
    {
        private BaseResources() { }

        private BaseResources(String cookie)
        {
            _user = SessionHub.GetUserFromCookie(cookie);
        }

        private String _baseCSS =
            @"<link rel='stylesheet' href='Styles/metro.css' />
    <link rel='stylesheet' href='Styles/metro-icons.css' />
    <link rel='stylesheet' href='Styles/metro-colors.css' />
    <link rel='stylesheet' href='Styles/metro-schemes.css' />
    <link rel='stylesheet' href='Styles/Custom.css' />";
        private String _baseJS =
            @"<script src='Scripts/Lib/jquery.js'></script>
    <script src='Scripts/Lib/metro.js'></script>
    <script src='Scripts/Lib/angular.js'></script>
    <script src='Scripts/Lib/angular-cookies.js'></script>
    <script src='Scripts/Lib/angular-websocket.js'></script>
    <script src='Scripts/Lib/angular-uuid.js'></script>
    <script src='Scripts/Bootstrapper.js'></script>
    <script src='Scripts/Services/ClientStore.js'></script>
    <script src='Scripts/Directives.js'></script>";


        private String _favicon =
            @"<link rel='icon' href='Images/Lotus.ico' type='image/ico' sizes='16x16'>";

        private String _header =
            @"<header class='app-bar darcula'>
        <div class='container' ng-controller='BootstrapperController'>
            <a class='app-bar-element branding' href='/'>
                <img src = 'Images/Lotus.png' style='height: 42px; margin-right:8px;' />
                <span style = 'font-size: 1.1rem' > Lotus Administration</span>
            </a>
            <span class='app-bar-divider'></span>
            <ul class='app-bar-menu'>
                <li><a class='app-bar-element' href='/GetStarted' ng-hide='authenticated'>Get Started</a></li>
                <li><a class='app-bar-element' href='/ControlPanel' ng-show='authenticated'>Control Panel</a></li>
                <li data-flexorderorigin='2' data-flexorder='3'>
                    <a href='#' class='dropdown-toggle'>About</a>
                    <ul class='d-menu' data-role='dropdown' data-no-close='true'>
                        <li><a href='/Features'>Features</a></li>
                        <li><a href='/FAQ'>FAQ</a></li>
                        <li><a href='/ContactUs'>Contact Us</a></li>
                    </ul>
                </li>
                <li><a class='app-bar-element fg-red' href='/Premium'>Premium</a></li>
            </ul>
            <a class='app-bar-element place-right' href='/Register' ng-hide='authenticated'>Register</a>
            <a class='app-bar-element place-right' href='/Login' ng-hide='authenticated'>Login</a>
            <a class='app-bar-element place-right' ng-show='authenticated' ng-click='logout()'>Logout</a>
            <a class='app-bar-element place-right' ng-show='authenticated'>Logged in as {{email}}</a>
        </div>
    </header>";

        private User _user = null;

        public String BaseCSS
        {
            get
            {
                return _baseCSS;
            }
        }

        public String UserInfo
        {
            get
            {
                if (_user == null)
                {
                    return "<script>var authenticated = false; var email = null;</script>";
                }
                else
                {
                    return "<script>var authenticated = true; var email = '" + _user.Email + "';</script>";
                }
            }
        }

        public String BaseJS
        {
            get
            {
                return _baseJS;
            }
        }

        public String Favicon
        {
            get
            {
                return _favicon;
            }
        }

        public String Header
        {
            get
            {
                return _header;
            }
        }

        public static BaseResources GetResources(HttpListenerContext context)
        {
            Cookie session = context.Request.Cookies["LOTUS_SESSION_ID"];
            if (session != null)
            {
                return new BaseResources(session.Value);
            }
            else
            {
                return new BaseResources();
            }
        }
    }
}
