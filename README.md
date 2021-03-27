# ModelToView

Generates localizable HTML form elements using custom attributes annotated view models.

## Getting started

### Cloning the repository

`git clone https://github.com/bigabdoul/ModelToView.git`

Open the solution file in `.\Carfamsoft.ModelToView.sln`. In Visual Studio, make
sure that the sample project in `.\src\Samples\Web\Carfamsoft.ModelToView.Samples.Web.AutoRazorViews`
is set as the startup project. Make sure to clean and rebuild the solution.
Press `Ctrl+F5` to launch the project. Navigate to the URL `/Account/Register`
and try out different views. Have fun exploring these source files:

- `/Views/Account/Login.cshtml`,
- `/Views/Account/Register.cshtml`,
- `/Views/Account/Update.cshtml`,
- `/Views/Shared/_AutoEditView.cshtml`
- `/Views/Shared/_AutoInputView.cshtml`
- `/Views/Shared/_AutoInputGroupView.cshtml`

They show you how to do custom view model rendering.

### Using NuGet

`Install-Package Carfamsoft.ModelToView.Mvc`

If you get errors like:
- `The type 'Type' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'`
You may resolve this problem by reading this issue: https://github.com/dotnet/standard/issues/542

- `Could not find a part of the path '...\ModelToView\src\Samples\Web\AutoRazorViews\bin\roslyn\csc.exe'.` In Visual Studio you may resolve this error by going to the `Build` menu:
  - Build > Clean Solution
  - Build > Rebuild Solution

Refresh your browser page with the error and it should all be fine again.

### An AngularJS sample

In a Razor view:

```HTML
@using Carfamsoft.ModelToView.Mvc
@using MyApp.Web.Models

<div class="registration-form-container" ng-controller="RegistrationController">
    @* To use an ordinary form omit the ngModel parameter. *@
    @(Html.AutoEditForm(new RegisterBusinessModel()
        , ngModel: "model"
        , formName: "registrationForm"
        , additionalHtml: AdditionalContent()
        , generateNameAttributes: true))
</div>

@helper AdditionalContent()
{
    <div class="col-md-12">
        <div class="form-group">
            <span class="checkbox">
                <label>
                    <input type="checkbox" ng-model="model.accept" />
                    <span></span>
                    By submitting this form, I declare that I have read and accepted the terms and conditions of use.
                </label>
            </span>
        </div>
    </div>
    <div class="col-md-3">
        <div class="form-group">
            <button type="submit" class="btn btn-primary" ng-disabled="busy">
                <i class="fa fa-send"></i> Submit
            </button>
        </div>
    </div>
}

<script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.8.2/angular.min.js"></script>
<script>
    var myapp = angular.module("myapp", ["ngAnimate", "ngSanitize"]);
    myapp.controller("RegistrationController", ["$scope", "$http", function ($scope, $http) {
        $scope.model = {};

        $scope.submitForm = function () {
            $scope.busy = true;

            $http.post("@Url.Action("register")").then(function (result) {
                $scope.busy = false;
                // do something
            }).catch(function (err) {
                $scope.busy = false;
                console.error(err);
                // handle error
            });
        };
    }
</script>
```

#### The business registration model:

```C#
using Carfamsoft.ModelToView.Mvc;
using Carfamsoft.ModelToView.ViewAnnotations;
using Carfamsoft.ModelToView.WebPages;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Models
{
    // DisplayStrings is a type defined in another assembly. It provides localized strings.
    [FormDisplayDefault(ResourceType = typeof(DisplayStrings))]
    public class RegisterBusinessModel
    {
        /*********** Authentication ***********/

        [Required]
        [FormDisplay(GroupName = "Authentication", Order = 1,Icon = "fas fa fa-user")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6, ErrorMessageResourceName = "PasswordLengthError", ErrorMessageResourceType = typeof(DisplayStrings))]
        [DataType(DataType.Password)]
        [FormDisplay(GroupName = "Authentication", Order = 2, Icon = "fas fa fa-lock")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [FormDisplay(GroupName = "Authentication", Order = 3, Icon = "fas fa fa-lock")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.", ErrorMessageResourceName = "ConfirmPasswordError", ErrorMessageResourceType = typeof(DisplayStrings))]
        public string ConfirmPassword { get; set; }

        /*********** Personal Info ***********/

        [Required]
        [FormDisplay(GroupName = "PersonalInfo", Order = 1, Icon = "fas fa fa-user")]
        public string FirstName { get; set; }

        [Required]
        [FormDisplay(GroupName = "PersonalInfo", Order = 2, Icon = "fas fa fa-user")]
        public string LastName { get; set; }

        [FormDisplay(GroupName = "PersonalInfo", Order = 3, Tag = "select", Options = "=Select|F=Female|M=Male")]
        public string Gender { get; set; }

        /*********** Contact ***********/

        [Required, StringLength(56)]
        [FormDisplay(GroupName = "Contact", Order = 1, Icon = "fas fa fa-envelope")]
        public string Email { get; set; }

        [StringLength(15)]
        [FormDisplay(GroupName = "Contact", Order = 2, Icon = "fas fa fa-mobile")]
        public string MobilePhone { get; set; }

        [StringLength(15)]
        [FormDisplay(GroupName = "Contact", Order = 3, Icon = "fas fa fa-phone")]
        public string HomePhone { get; set; }

        /*********** Address ***********/

        [FormDisplay(GroupName = "Address", Order = 1, Icon = "fas fa fa-globe-africa")]
        public int? CountryId { get; set; }

        [StringLength(50)]
        [FormDisplay(GroupName = "Address", Order = 2, Icon = "fas fa fa-city")]
        public string City { get; set; }

        [StringLength(100)]
        [FormDisplay(GroupName = "Address", Order = 3, Tag = "textarea", Icon = "fas fa fa-home")]
        public string Address { get; set; }

        [DisplayIgnore]
        public string Token { get; set; }
    }
}
```

### Sample view model annotations

```C#
using Carfamsoft.ModelToView.Testing.Resources;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Carfamsoft.ModelToView.Testing
{
    [FormDisplayDefault(ShowGroupName = true, ResourceType = typeof(DisplayStrings))]
    public class UpdateUserModel
    {
        [Required]
        [StringLength(100)]
        [FormDisplay(GroupName = "PersonalInfo", Icon = "fas fa-user")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [FormDisplay(GroupName = "PersonalInfo", Icon = "fas fa-user")]
        public string LastName { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        [FormDisplay(GroupName = "ContactDetails", Icon = "fas fa-envelope", UITypeHint = "email")]
        public string Email { get; set; }

        [StringLength(30)]
        [FormDisplay(GroupName = "ContactDetails", Icon = "fas fa-phone", UITypeHint = "phone")]
        public string PhoneNumber { get; set; }
    }

    public class AutoUpdateUserModel : UpdateUserModel
    {
        [DisplayIgnore]
        public string Id { get; set; }

        [Range(typeof(DayOfWeek), "Monday", "Friday")]
        [FormDisplay(GroupName = "PleaseSelect", Tag = "select", Name = "", Order = 1, Prompt = nameof(FavouriteWorkingDay), Icon = "fas fa-calendar")]
        public string FavouriteWorkingDay { get; set; }

        [FormDisplay(GroupName = "PleaseSelect", UIHint = "select", Name = "", Order = 2)]
        public int AgeRange { get; set; }

        //[Range(typeof(ConsoleColor), "Black", "White")]
        [FormDisplay(Type = "radio", Order = 3, Options = "Black|Blue|White")]
        public string FavouriteColor { get; set; }

        [FormDisplay(Order = 4, Description = "LoginWithEmailAndSms")]
        public bool TwoFactorEnabled { get; set; }
    }
}
```

### Sample unit test

```C#
using Carfamsoft.ModelToView.Mvc;
using Carfamsoft.ModelToView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Globalization;
using static System.Threading.Thread;

namespace Carfamsoft.ModelToView.Testing.Tests
{
    [TestClass]
    public class NestedTagBuilderTest
    {
        [TestMethod]
        public void Should_Render_AutoEditForm()
        {
            // arrange
            CurrentThread.CurrentUICulture = new CultureInfo("fr");

            var model = new AutoUpdateUserModel
            {
                FirstName = "Abdoul",
                LastName = "Kaba",
                Email = "abdoul.kaba@example.com",
                PhoneNumber = "22312345678",
            };
            var formAction = "https://example.com/register";
            var form = new FormAttributes
            {
                Id = "registerUserForm",
                Action = formAction,
            };

            var renderOptions = new ControlRenderOptions
            {
                CamelCaseId = true,
                GenerateIdAttribute = true,
                GenerateNameAttribute = true,
                OptionsGetter = propertyName =>
                {
                    if (propertyName.Equals(nameof(AutoUpdateUserModel.AgeRange)))
                    {
                        return new []
                        {
                            new SelectOption(id: 0, value: "[Your most appropriate age]", isPrompt: true),
                            new SelectOption(1, "Minor (< 18)"),
                            new SelectOption(2, "Below or 25"),
                            new SelectOption(3, "Below or 30"),
                            new SelectOption(4, "Below or 40"),
                            new SelectOption(5, "Below 50"),
                            new SelectOption(6, "Between 50 and 54"),
                            new SelectOption(7, "Between 55 and 60"),
                            new SelectOption(8, "Above 60"),
                            new SelectOption(9, "Above 70"),
                            new SelectOption(10, "Above 80"),
                        };
                    }
                    return null;
                }
            };

            // act

            var result = form.RenderAutoEditForm(model, renderOptions).RenderButton(text: "&nbsp;Save", config: button =>
            {
                button.AddChild(NestedTagBuilder.Create("i").AddClass("fas fa fa-save"));
            }).ToString();

            Debug.WriteLine(result);

            // assert

            Assert.IsTrue(true == result?.Contains($"action=\"{formAction}\""));
            Assert.IsTrue(result.Contains("abdoul.kaba@example.com"));
        }
    }
}
```
