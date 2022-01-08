namespace BlazorApp.Domain.Identity;

public class Permissions
{
    public static class Identity
    {
        public const string Register = "Permissions.Identity.Register";
    }

    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string ListAll = "Permissions.Roles.ViewAll";
        public const string Register = "Permissions.Roles.Register";
        public const string Update = "Permissions.Roles.Update";
        public const string Remove = "Permissions.Roles.Remove";
    }

    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Search = "Permissions.RoleClaims.Search";
    }

    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Export = "Permissions.Users.Export";
        public const string Search = "Permissions.Users.Search";
    }

    public static class Dashboard
    {
        public const string View = "Permissions.Dashboard.View";
    }

    public static class Accounts
    {
        public const string View = "Permissions.Accounts.View";
        public const string Search = "Permissions.Accounts.Search";
        public const string Register = "Permissions.Accounts.Register";
        public const string Update = "Permissions.Accounts.Update";
        public const string Remove = "Permissions.Accounts.Remove";
    }

    public static class Transactions
    {
        public const string View = "Permissions.Transactions.View";
        public const string Search = "Permissions.Transactions.Search";
        public const string Register = "Permissions.Transactions.Register";
        public const string Update = "Permissions.Transactions.Update";
        public const string Remove = "Permissions.Transactions.Remove";
    }
}