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
        public void Should_Render_Object_As_Html()
        {
            // arrange
            CurrentThread.CurrentUICulture = new CultureInfo("en");

            var model = GetRegisterBusinessModel();
            var container = NestedTagBuilder.Create("div");

            // act
            var result = container.RenderAsHtmlString(model);

            Debug.WriteLine(result);

            // assert

            Assert.IsTrue(true == result?.Contains("First name"));
        }

        [TestMethod]
        public void Should_Render_Object_As_Html_French()
        {
            // arrange
            CurrentThread.CurrentUICulture = new CultureInfo("fr");

            var model = GetRegisterBusinessModel();
            var container = NestedTagBuilder.Create("div");

            // act
            var result = container.RenderAsHtmlString(model);

            Debug.WriteLine(result);

            // assert

            Assert.IsTrue(true == result?.Contains("Prénom(s)"));
        }

        [TestMethod]
        public void Should_Render_Object_Without_Attributes()
        {
            // arrange
            var model = GetRegisterModel();
            var container = NestedTagBuilder.Create("div");

            // act
            var result = container.RenderAsHtmlString(model, "model");

            Debug.WriteLine(result);

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }

        [TestMethod]
        public void Should_RenderAsHtmlString()
        {
            // arrange
            var model = GetRegisterUserModel();
            var container = NestedTagBuilder.Create("div");

            // act
            var result = container.RenderAsHtmlString(model, "model");

            Debug.WriteLine(result);

            // assert

            Assert.IsTrue(true == result?.Contains("abdoul.kaba@example.com"));
        }

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
                PhoneNumber = "92469522",
            };
            var formAction = "https://example.com/register";
            var form = new FormAttributes
            {
                Id = "registerUserForm",
                Action = formAction,
                //OptionsGetter = null,
                //DisabledGetter = null,
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

        static RegisterUserModel GetRegisterUserModel() => new RegisterUserModel
        {
            FirstName = "Abdoul",
            LastName = "Kaba",
            Email = "abdoul.kaba@example.com",
            Password = "123456",
            ConfirmPassword = "123456",
            PhoneNumber = "92469522",
        };

        static RegisterModel GetRegisterModel() => new RegisterModel
        {
            UserName = "bigabdoul",
            FirstName = "Abdoul",
            LastName = "Kaba",
            Gender = "M",
            Email = "abdoul.kaba@example.com",
            CountryId = 223,
            City = "Bamako",
            Address = "Adeken",
            Password = "123456",
            ConfirmPassword = "123456",
            MobilePhone = "92469522",
        };

        static RegisterBusinessModel GetRegisterBusinessModel() => new RegisterBusinessModel
        {
            UserName = "bigabdoul",
            FirstName = "Abdoul",
            LastName = "Kaba",
            Gender = "M",
            Email = "abdoul.kaba@example.com",
            CountryId = 223,
            City = "Bamako",
            Address = "Adeken",
            Password = "123456",
            ConfirmPassword = "123456",
            MobilePhone = "92469522",
        };
    }
}
