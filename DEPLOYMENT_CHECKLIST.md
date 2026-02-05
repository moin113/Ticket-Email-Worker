# AWS Lambda Deployment Checklist

## Pre-Deployment ✅
- [x] Code changes completed
- [x] Release build successful
- [x] Email template updated with:
  - CodeToSolutions branding
  - Passenger details table
  - Refund amount display
  - Professional design

## Deployment Steps

### Step 1: Right-click Project in Visual Studio
1. Open Visual Studio
2. In Solution Explorer, right-click on **TicketEmailWorker** project
3. Select **Publish to AWS Lambda...**

### Step 2: Select Existing Function
1. In the publish dialog, select **Existing Lambda Function**
2. Choose your function name: `TicketEmailWorker` (or your actual function name)
3. Click **Next**

### Step 3: Configure Settings
1. **Function Handler**: Should be `TicketEmailWorker::TicketEmailWorker.Function::FunctionHandler`
2. **Runtime**: .NET 6 or .NET 8 (depending on your project)
3. **Memory**: Keep existing settings
4. **Timeout**: Keep existing settings (should be at least 30 seconds for email sending)
5. Click **Upload**

### Step 4: Wait for Upload
- Monitor the upload progress
- Wait for "Upload successful" message
- ✅ **Upload Successful**: _____ (check when done)

## Post-Deployment Verification

### Step 5: Test the Function
1. Go to AWS Lambda Console
2. Navigate to your `TicketEmailWorker` function
3. Click **Test** tab
4. Use existing test event or create new one with:
```json
{
  "TicketDetailId": <your_test_ticket_id>
}
```
5. Click **Test** button

### Step 6: Check CloudWatch Logs
1. Go to **Monitor** tab in Lambda console
2. Click **View CloudWatch logs**
3. Open the latest log stream
4. Verify the following log entries:

**Expected Success Logs:**
```
✅ TicketEmailWorker started for TicketDetailId = X
✅ Ticket fetched: Username=..., Email=...
✅ Admin ToEmail = mohammed.moinashraf@gmail.com
✅ Sending Mail To = <user_email>
✅ Sending Mail From = support@codetosolutions.com
✅ Admin email sent
✅ User ToEmail = <user_email>
✅ User email sent
✅ All cancellation emails sent successfully!
```

**Check for Errors:**
- ❌ No "System.MissingMethodException"
- ❌ No "FlightAPI Error"
- ❌ No template rendering errors
- ❌ No SMTP errors

✅ **CloudWatch Logs Clean**: _____ (check when verified)

### Step 7: Verify Email Delivery
1. Check admin email: `mohammed.moinashraf@gmail.com`
2. Check user email (the email from test ticket)
3. Verify email contains:
   - ✅ CodeToSolutions branding
   - ✅ PNR Number
   - ✅ Passenger details table
   - ✅ Flight information
   - ✅ Refund amount in green box
   - ✅ All formatting looks correct

## Troubleshooting

### If Upload Fails:
- Check AWS credentials are configured
- Verify IAM permissions for Lambda deployment
- Check network connectivity

### If Function Fails:
- Check CloudWatch logs for specific error
- Verify appsettings.json is included in deployment
- Check Resource.resx is embedded properly
- Verify all NuGet packages are restored

### If Email Not Sent:
- Check SMTP settings in appsettings.json
- Verify email service credentials
- Check network/firewall settings
- Review CloudWatch logs for SMTP errors

## Environment Variables to Verify (if using)
- `FlightAdmin:BaseURL`
- `FlightAdmin:EmailConfirmFromEmail`
- `FlightAdmin:EmailConfirmToEmail`
- `SmtpSettings:Server`
- `SmtpSettings:Port`
- `SmtpSettings:Username`
- `SmtpSettings:Password`

## Final Checklist
- [ ] Upload successful
- [ ] CloudWatch logs show no errors
- [ ] Admin email received with new template
- [ ] User email received with new template
- [ ] All passenger details displayed correctly
- [ ] Refund amount displayed correctly
- [ ] Brand name "CodeToSolutions" visible
- [ ] Professional design renders correctly

---

## Deployment Date: _____________________
## Deployed By: _____________________
## Function Name: _____________________
## AWS Region: _____________________

**Status**: ⬜ Pending | ⬜ In Progress | ⬜ Completed | ⬜ Failed
