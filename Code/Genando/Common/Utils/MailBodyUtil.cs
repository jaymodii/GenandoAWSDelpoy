namespace Common.Utils;

public class MailBodyUtil
{
    private const string header = @"
<head>
  <style>
    /* Reset some default styles for better email rendering */
    body, div, p {
      margin: 0;
      padding: 0;
    }

    /* Main styles for the email */
    .email-container {
      font-family: Arial, sans-serif;
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
      border: 1px solid #ccc;
      background-color: #f4f4f4;
    }

    .email-header {
      background-color: #007bff;
      color: #fff;
      text-align: center;
      padding: 10px 0;
    }

    .email-content {
      padding: 20px;
      background-color: #fff;
    }

    .email-footer {
      text-align: center;
      padding: 10px 0;
      color: #777;
    }
  </style>
</head>";

    private const string footer = $@" <div class='email-footer'>
      <p>This email was sent from Genando.</p>
    </div>";

    public static string CreateNewAccountNotification(string username,
        string email,
        string password)
    {
        string body = $@"<div class='email-container'>
    <div class='email-header'>
      <h1>Welcome to Our Community!</h1>
    </div>
    <div class='email-content'>
      <p>Dear {username},</p>
      <p>Welcome to Genando. Your account has been successfully created.</p>
      <p>Here are your account details:</p>
      <ul>
        <li><strong>Username:</strong> {email}</li>
        <li><strong>Password:</strong> {password}</li>
      </ul>
      <p>Thank you for joining us!</p>
    </div>
  </div>";
        return CreateMessage(body);
    }

    public static string SendOtpForAuthenticationBody(string otp)
    {
        string body = $@"<div class='email-container'>
    <div class='email-header'>
      <h1>Welcome to Our Community!</h1>
    </div>
    <div class='email-content'>
      <p>Dear User,</p>
      <p>Here is the otp for authentication and it is valid for 10 minutes only:</p>
      <ul>
        <li><strong>OTP:</strong> {otp}</li>
      </ul>
      <p>Thank you for using our service !</p>
    </div>
  </div>";
        return CreateMessage(body);
    }

    public static string SendOtpForProfileBody(string otp)
    {
        string body = $@"<div class='email-container'>
        <div class='email-header'>
            <h1>Profile Update OTP</h1>
        </div>
        <div class='email-content'>
            <p>Dear User,</p>
            <p>Your OTP for profile update is:</p>
        <ul>
            <li><strong>OTP:</strong> {otp}</li>
        </ul>
            <p>This OTP is valid for 10 minutes. Please use it to complete your profile update.</p>
            <p>Thank you for using our service!</p>
        </div>
    </div>";

        return CreateMessage(body);
    }


    public static string SendResetPasswordLink(string link)
    {
        string body = $@"<div class='email-container'>
    <div class='email-header'>
      <h1>Welcome to Our Community!</h1>
    </div>
    <div class='email-content'>
      <p>Dear User,</p>
      <p>Here is link for resetting your account password and it is valid for 10 minutes only:</p>
      <ul>
        <li>{link}</li>
      </ul>
      <p>Thank you for using our service !</p>
    </div>
  </div>";


        return CreateMessage(body);
    }

    private static string CreateMessage(string body)
    {
        return $@"<!DOCTYPE html>
                <html>
                    {header}
                    <body>
                        {body}
                        {footer}
                    </body>
                </html>
                ";
    }
    public static string SendTestReportEmail(string patientName, string doctorName, string reportLink, string notes)
    {
        // Ensure that the notes are properly decoded from HTML-encoded format
        notes = System.Net.WebUtility.HtmlDecode(notes);

        string body = $@"<div class='email-container'>
        <div class='email-header'>
            <h1>Medical Test Report</h1>
        </div>
        <div class='email-content'>
            <p>Dear <b>{patientName}</b>,</p>
            <br>
            <p>   We are pleased to provide you with your medical test report, as prepared by <b>Dr.{doctorName}</b>.</p><br>
          
            <p>Dr.{doctorName}'s views on Report:</p><br>
            <div style='background-color: #f7f7f7; padding: 10px; border: 1px solid #ddd;'>{notes}</div><br>

            <p><b>Please find the attached PDF report for your reference </b></p><br>
            
            <p>If you have any questions or need further assistance, please do not hesitate to contact us.</p><br>
            <p>Thank you for choosing Genando for your healthcare needs.</p>
        </div>
    </div>";

        return CreateMessage(body);
    }

    public static string SendPatientDetailsUpdatedEmail(string patientName, string email, DateTimeOffset dateOfBirth, string phoneNumber, string gender, string address, string doctorName)
    {
        string body = $@"<div class='email-container'>
        <div class='email-header'>
            <h1>Updated Patient Details</h1>
        </div>
        <div class='email-content'>
            <p>Dear <b>{patientName}</b>,</p>
            <br>
            <p>Your details have been updated:</p>
            <ul>
                <li><b>Name:</b> {patientName}</li>
                <li><b>Email:</b> {email}</li>
                <li><b>Date of Birth:</b> {dateOfBirth.ToString("yyyy-MM-dd")}</li>
                <li><b>Phone Number:</b> {phoneNumber}</li>
                <li><b>Gender:</b> {gender}</li>
                <li><b>Address:</b> {address}</li>
            </ul>
            <br>
            <p>If any of the above information is incorrect , please contact Dr.<b>{doctorName}</b></p><br>

            <p>Thank you for choosing Genando for your healthcare needs.</p>
        </div>
    </div>";

        return CreateMessage(body);
    }
}