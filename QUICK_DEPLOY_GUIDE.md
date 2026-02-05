# Quick Deployment Guide - Visual Studio to AWS Lambda

## ✅ Pre-Deployment Status
- **Build Status**: ✅ Release build successful
- **Configuration**: Release
- **Runtime**: .NET 8
- **Region**: ap-south-1 (Mumbai)
- **Memory**: 512 MB
- **Timeout**: 30 seconds
- **Handler**: TicketEmailWorker::TicketEmailWorker.Function::FunctionHandler

---

## 🚀 Deployment Steps (Visual Studio)

### 1️⃣ Open Solution in Visual Studio
- Open `TicketEmailWorker.sln`

### 2️⃣ Right-Click Project
- In **Solution Explorer**, right-click on **TicketEmailWorker** project (not the solution)
- Select **Publish to AWS Lambda...**

### 3️⃣ AWS Credentials
- Ensure you're logged in with correct AWS credentials
- Profile: `default`
- Region: `ap-south-1`

### 4️⃣ Select Function
- Choose **Existing Lambda Function**
- Select your function from the dropdown (likely named `TicketEmailWorker`)

### 5️⃣ Review Settings
The wizard should auto-populate from `aws-lambda-tools-defaults.json`:
- **Function Handler**: `TicketEmailWorker::TicketEmailWorker.Function::FunctionHandler`
- **Runtime**: .NET 8
- **Memory**: 512 MB
- **Timeout**: 30 seconds
- **Configuration**: Release

### 6️⃣ Click Upload
- Click the **Upload** or **Publish** button
- Wait for the deployment to complete
- Look for "✅ Upload successful" message

---

## 🔍 Post-Deployment Verification

### Step 1: Check AWS Lambda Console
1. Open AWS Console → Lambda
2. Navigate to your function
3. Verify **Last modified** timestamp is recent

### Step 2: Test the Function
**Option A: Using AWS Console**
1. Click **Test** tab
2. Create/use test event:
```json
{
  "TicketDetailId": 123
}
```
3. Click **Test**
4. Check execution result

**Option B: Using dotnet CLI**
```powershell
dotnet lambda invoke-function TicketEmailWorker --payload "{\"TicketDetailId\":123}"
```

### Step 3: Monitor CloudWatch Logs
1. Go to **Monitor** tab
2. Click **View CloudWatch logs**
3. Open latest log stream
4. Look for:

**✅ Success Indicators:**
```
✅ TicketEmailWorker started for TicketDetailId = 123
✅ Ticket fetched: Username=..., Email=...
✅ Admin email sent
✅ User email sent
✅ All cancellation emails sent successfully!
```

**❌ Error Indicators to Watch For:**
```
❌ Invalid request received
❌ TicketDetails not found
❌ FlightAPI Error
❌ Exception Occurred
❌ SMTP errors
```

### Step 4: Verify Email Content
Check both admin and user emails for:
- ✅ **CodeToSolutions** brand header (blue gradient)
- ✅ Cancellation confirmation notice (yellow box)
- ✅ PNR Number (highlighted)
- ✅ Passenger details table with all passengers
- ✅ Flight route with visual arrow
- ✅ **Refund amount** in large green box
- ✅ Payment summary section
- ✅ Professional footer with contact info

---

## 🐛 Troubleshooting

### Upload Fails
**Error**: "Unable to connect to AWS"
- **Fix**: Check AWS credentials configuration
- Run: `aws configure list` to verify

**Error**: "Access Denied"
- **Fix**: Verify IAM permissions include `lambda:UpdateFunctionCode`

### Function Fails After Deployment
**Error**: "Could not load file or assembly"
- **Fix**: Clean and rebuild in Release mode
- Delete `bin/` and `obj/` folders, then rebuild

**Error**: "appsettings.json not found"
- **Fix**: Ensure `appsettings.json` has:
  - Build Action: `Content`
  - Copy to Output Directory: `Copy if newer`

**Error**: "Resource.resx not found"
- **Fix**: Ensure `Resource.resx` has:
  - Build Action: `Embedded Resource`
  - Custom Tool: `ResXFileCodeGenerator`

### Email Template Not Rendering
**Error**: Mustache template errors in logs
- **Fix**: Check Resource.resx was properly embedded
- Rebuild project and redeploy

---

## 📋 Deployment Checklist

- [ ] Visual Studio opened with solution loaded
- [ ] Right-clicked TicketEmailWorker project
- [ ] Selected "Publish to AWS Lambda"
- [ ] Selected existing function
- [ ] Verified settings are correct
- [ ] Clicked Upload/Publish
- [ ] Saw "Upload successful" message
- [ ] Tested function in AWS Console
- [ ] Checked CloudWatch logs - no errors
- [ ] Verified admin email received
- [ ] Verified user email received
- [ ] Confirmed new template displays correctly
- [ ] Confirmed passenger details shown
- [ ] Confirmed refund amount shown

---

## 📞 Support

If you encounter issues:
1. Check CloudWatch logs first
2. Verify all environment variables in Lambda configuration
3. Test locally using Lambda Test Tool before deploying
4. Review `DEPLOYMENT_CHECKLIST.md` for detailed troubleshooting

---

**Deployment Date**: _______________
**Status**: ⬜ Ready | ⬜ In Progress | ⬜ Complete | ⬜ Failed
