using Authentication.Controllers;
using Authentication.Models;
using Authentication.Models.DTOs;
using Authentication.Services.Interfaces;

using FakeItEasy;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace AuthUnitTest.Controller
{
    public class UserControllerTest
    {
        private readonly IUserService _userService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _userService = A.Fake<IUserService>();
            _userController = new UserController(_userService);
        }

        //private static UserRegistrationModel CreateUserModel() => A.Fake<UserRegistrationModel>();

        [Fact]
        public async void UserController_CreateUser_ReturnSuccess()
        {
            // Arrange
            var user = new UserRegistrationModel()
            {
                Email = "test@email.com",
                FirstName = "fName",
                LastName = "lName",
                Mobile = "1234567890"
            };
            var response = new ResponseDto()
            {
                Success = true,
                StatusCode = HttpStatusCode.OK
            };

            //Act
            A.CallTo(() => _userService.CreateUser(user)).Returns(response);
            var result = (OkObjectResult)await _userController.CreateUser(user);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("User0001", "Test@1234")]
        public async void UserController_Login_ReturnSuccess(string username, string password)
        {
            //Arrange
            var response = new ResponseDto<LoginDto>()
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Data = A.Fake<LoginDto>()
            };

            //Act
            A.CallTo(() => _userService.Authentication(username, password)).Returns(response);
            var result = (OkObjectResult)await _userController.Login(username, password);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
