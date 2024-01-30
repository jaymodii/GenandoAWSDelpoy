export class TabTitleConstant {
    public static genando = "Genando/";
    //authentication routing path
    public static login = this.genando + "Login";
    public static verifyOtp = this.genando + "Verify-Otp";
    public static forgotPassword = this.genando + "Forgot-Password";
    public static resetPassword = this.genando + "Reset-Password";

    //For doctor user
    public static readonly register : string = this.genando + "Register";
    public static readonly dashboard : string = this.genando + "Dashboard";
}
