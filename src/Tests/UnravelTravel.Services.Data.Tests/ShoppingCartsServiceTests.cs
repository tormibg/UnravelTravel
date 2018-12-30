﻿namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using Xunit;

    public class ShoppingCartsServiceTests : BaseServiceTests
    {
        private const string TestUsername = "Tester";
        private const string SecondTestUsername = "Prokop";

        private const int TestShoppingCartId = 1;

        private const int TestShoppingCartActivityId = 1;
        private const int TestShoppingCartActivityQuantity = 2;
        private const int NewShoppingCartActivityQuantity = 4;
        private const int SecondTestShoppingCartActivityId = 2;

        private const int TestActivityId = 1;
        private const string TestActivityName = "Test Activity 123";
        private const decimal TestActivityPrice = 12.55m;
        private const int TestLocationId = 1;
        private const string TestLocationName = "Test Location 123";
        private const int SecondTestActivityId = 2;
        private const string SecondTestActivityName = "Secondd Activity";
        private const decimal SecondTestActivityPrice = 50.80m;

        private const string TestImageUrl = "https://someurl.com";

        private readonly string testUserId = Guid.NewGuid().ToString();
        private readonly string secondTestUserId = Guid.NewGuid().ToString();
        private readonly DateTime testDate = DateTime.Now.AddDays(2);


        private IShoppingCartsService ShoppingCartsServiceMock => this.ServiceProvider.GetRequiredService<IShoppingCartsService>();

        [Fact]
        public async Task AssignShoppingCartsUserIdThrowsNullReferenceExceptionIfNoShoppingCartForThisUser()
        {
            var user = new ApplicationUser { Id = this.secondTestUserId, UserName = SecondTestUsername, };
            await this.DbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ShoppingCartsServiceMock.AssignShoppingCartsUserId(user));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceShoppingCartForUser, user.Id, user.UserName), exception.Message);
        }

        [Fact]
        public async Task AssignShoppingCartsUserIdAssignsShoppingCartToUser()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            var user = this.DbContext.Users.Find(testUserId);
            await this.ShoppingCartsServiceMock.AssignShoppingCartsUserId(user);
            var shoppingCart = this.DbContext.ShoppingCarts.Find(TestShoppingCartId);
            Assert.Equal(shoppingCart.UserId, user.Id);
        }

        [Fact]
        public async Task GetAllShoppingCartActivitiesAsyncAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ShoppingCartsServiceMock.GetAllShoppingCartActivitiesAsync(SecondTestUsername));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, SecondTestUsername), exception.Message);
        }

        [Fact]
        public async Task GetAllShoppingCartActivitiesAsyncReturnsCorrectShoppingCart()
        {
            await GetAllShoppingCartActivitiesSetDbContext();
            var expected = GetAllShoppingCartActivitiesExpectedResult();
            var actual = await this.ShoppingCartsServiceMock.GetAllShoppingCartActivitiesAsync(TestUsername);
            Assert.Collection(actual,
                elem1 =>
            {
                Assert.Equal(expected[0].Id, elem1.Id);
                Assert.Equal(expected[0].ActivityId, elem1.ActivityId);
                Assert.Equal(expected[0].ActivityName, elem1.ActivityName);
                Assert.Equal(expected[0].ActivityLocationName, elem1.ActivityLocationName);
                Assert.Equal(expected[0].ActivityPrice, elem1.ActivityPrice);
                Assert.Equal(expected[0].ShoppingCartUserId, elem1.ShoppingCartUserId);
                Assert.Equal(expected[0].Quantity, elem1.Quantity);
            },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].ActivityId, elem2.ActivityId);
                    Assert.Equal(expected[1].ActivityName, elem2.ActivityName);
                    Assert.Equal(expected[0].ActivityLocationName, elem2.ActivityLocationName);
                    Assert.Equal(expected[1].ActivityPrice, elem2.ActivityPrice);
                    Assert.Equal(expected[1].ShoppingCartUserId, elem2.ShoppingCartUserId);
                    Assert.Equal(expected[1].Quantity, elem2.Quantity);
                });
        }

        [Fact]
        public async Task ClearShoppingCartThrowsNullReferenceExceptionIfUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.ClearShoppingCart(TestUsername));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, TestUsername), exception.Message);
        }

        [Fact]
        public async Task ClearShoppingCartClearsUsersShoppingCart()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();
            await this.ShoppingCartsServiceMock.ClearShoppingCart(TestUsername);
            var actual = this.DbContext.ShoppingCarts.First(sc => sc.User.UserName == TestUsername).ShoppingCartActivities.Count(sca => sca.IsDeleted == false);
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task GetGuestShoppingCartActivityToAddThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingLocationToDb();
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.GetGuestShoppingCartActivityToAdd(SecondTestActivityId,
                    TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, SecondTestActivityId), exception.Message);
        }

        [Fact]
        public async Task GetGuestShoppingCartActivityToAddReturnsCorrectShoppingCartActivity()
        {
            await this.AddTestingLocationToDb();
            await this.AddTestingActivityToDb();

            var expected = new ShoppingCartActivityViewModel
            {
                ActivityId = TestActivityId,
                ActivityName = TestActivityName,
                ActivityDate = testDate,
                ActivityLocationName = TestLocationName,
                ActivityImageUrl = TestImageUrl,
                ActivityPrice = TestActivityPrice,
                Quantity = TestShoppingCartActivityQuantity,
            };
            var actual = await this.ShoppingCartsServiceMock.GetGuestShoppingCartActivityToAdd(TestActivityId,
                  TestShoppingCartActivityQuantity);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ActivityName, actual.ActivityName);
            Assert.Equal(expected.ActivityDate, actual.ActivityDate);
            Assert.Equal(expected.ActivityLocationName, actual.ActivityLocationName);
            Assert.Equal(expected.ActivityImageUrl, actual.ActivityImageUrl);
            Assert.Equal(expected.ActivityPrice, actual.ActivityPrice);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.IsType<ShoppingCartActivityViewModel>(actual);
        }

        [Fact]
        public async Task GetGuestShoppingCartActivityToAddThrowsArgumentExceptionIfNegativeOrZeroQuantity()
        {
            await this.AddTestingLocationToDb();
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ShoppingCartsServiceMock.GetGuestShoppingCartActivityToAdd(TestActivityId,
                    -1));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public void DeleteActivityFromGuestShoppingCartThrowsNullReferenceExceptionIfCartDoesNotContainActivity()
        {
            var cart = new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = TestShoppingCartActivityId,
                    ActivityId = TestShoppingCartId,
                    ActivityName = TestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = TestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
            };

            var exception = Assert.Throws<NullReferenceException>(() => this.ShoppingCartsServiceMock.DeleteActivityFromGuestShoppingCart(SecondTestShoppingCartActivityId,
                cart));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceGuestShoppingCartActivityId, SecondTestShoppingCartActivityId), exception.Message);
        }

        [Fact]
        public void DeleteActivityFromGuestShoppingCartThrowsNullReferenceExceptionIfActivityNotFound()
        {
            var cart = new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = TestShoppingCartActivityId,
                    ActivityId = TestShoppingCartId,
                    ActivityName = TestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = TestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
            };

            var exception = Assert.Throws<NullReferenceException>(() => this.ShoppingCartsServiceMock.DeleteActivityFromGuestShoppingCart(TestShoppingCartId,
                cart));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, TestShoppingCartId), exception.Message);
        }

        [Fact]
        public void DeleteActivityFromGuestShoppingCartDeletesShoppingCartActivityFromCart()
        {
            this.GetAllShoppingCartActivitiesSetDbContext().GetAwaiter().GetResult();
            var cart = this.GetAllShoppingCartActivitiesExpectedResult();

            cart = this.ShoppingCartsServiceMock.DeleteActivityFromGuestShoppingCart(TestShoppingCartId,
                 cart);

            Assert.Single(cart);
        }

        [Fact]
        public async Task DeleteActivityFromShoppingCartThrowsNullReferenceExceptionIfShoppingCartActivityNotFound()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            await this.AddTestingLocationToDb();
            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                LocationId = TestLocationId,
                Price = SecondTestActivityPrice,
            });
            this.DbContext.ShoppingCartActivities.Add(new ShoppingCartActivity
            {
                Id = TestShoppingCartActivityId,
                ShoppingCartId = TestShoppingCartId,
                ActivityId = TestActivityId,
                Quantity = TestShoppingCartActivityQuantity
            });
            await this.DbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.DeleteActivityFromShoppingCart(SecondTestShoppingCartActivityId, TestUsername));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceShoppingCartActivityId, SecondTestShoppingCartActivityId), exception.Message);
        }

        [Fact]
        public async Task DeleteActivityFromShoppingCartThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            await this.AddTestingLocationToDb();
            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                LocationId = TestLocationId,
                Price = SecondTestActivityPrice,
            });
            this.DbContext.ShoppingCartActivities.Add(new ShoppingCartActivity
            {
                Id = TestShoppingCartActivityId,
                ShoppingCartId = TestShoppingCartId,
                ActivityId = TestActivityId,
                Quantity = TestShoppingCartActivityQuantity
            });
            await this.DbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.DeleteActivityFromShoppingCart(TestShoppingCartActivityId, SecondTestUsername));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, SecondTestUsername), exception.Message);
        }

        [Fact]
        public async Task DeleteActivityFromShoppingCartDeletesShoppingCartActivity()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();
            await this.ShoppingCartsServiceMock.DeleteActivityFromShoppingCart(TestShoppingCartActivityId,
                TestUsername);
            var actual = this.DbContext.ShoppingCartActivities
                .Where(sca => sca.ShoppingCart.UserId == testUserId && sca.IsDeleted == false);
            Assert.Single(actual);
        }

        [Fact]
        public async Task AddActivityToShoppingCartAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            await this.AddTestingLocationToDb();
            this.DbContext.ShoppingCartActivities.Add(new ShoppingCartActivity
            {
                Id = TestShoppingCartActivityId,
                ShoppingCartId = TestShoppingCartId,
                ActivityId = TestActivityId,
                Quantity = TestShoppingCartActivityQuantity
            });
            await this.DbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.AddActivityToShoppingCartAsync(SecondTestActivityId, TestUsername,
                    TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, SecondTestActivityId), exception.Message);
        }

        [Fact]
        public async Task AddActivityToShoppingCartAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ShoppingCartsServiceMock.AddActivityToShoppingCartAsync(TestActivityId, SecondTestUsername,
                    TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, SecondTestUsername), exception.Message);
        }

        [Fact]
        public async Task AddActivityToShoppingCartAsyncThrowsArgumentExceptionIf()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ShoppingCartsServiceMock.AddActivityToShoppingCartAsync(TestActivityId, TestUsername,
                    0));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public async Task AddActivityToShoppingCartAsyncAddsActivityToShoppingCart()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            await this.AddTestingLocationToDb();
            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                LocationId = TestLocationId,
                Price = SecondTestActivityPrice,
            });
            await this.DbContext.SaveChangesAsync();

            await this.ShoppingCartsServiceMock.AddActivityToShoppingCartAsync(SecondTestActivityId, TestUsername,
                TestShoppingCartActivityQuantity);
            var actual = this.DbContext.ShoppingCartActivities
                .Where(sca => sca.ShoppingCart.User.UserName == TestUsername);
            Assert.Single(actual);
        }

        [Fact]
        public async Task EditShoppingCartActivityAsyncThrowsNullReferenceExceptionIfShoppingCartActivityNotFound()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ShoppingCartsServiceMock.EditShoppingCartActivityAsync(TestShoppingCartActivityId, TestUsername, TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceShoppingCartActivityId, TestShoppingCartActivityId), exception.Message);
        }

        [Fact]
        public async Task EditShoppingCartActivityAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ShoppingCartsServiceMock.EditShoppingCartActivityAsync(TestShoppingCartActivityId, secondTestUserId, TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, secondTestUserId), exception.Message);
        }

        [Fact]
        public async Task EditShoppingCartActivityAsyncThrowsArgumentExceptionIfZeroOrNegativeQuantity()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => this.ShoppingCartsServiceMock.EditShoppingCartActivityAsync(TestShoppingCartActivityId, TestUsername, -1));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public async Task EditShoppingCartActivityAsyncEditsShoppingCartActivity()
        {
            await this.GetAllShoppingCartActivitiesSetDbContext();

            await this.ShoppingCartsServiceMock.EditShoppingCartActivityAsync(TestShoppingCartActivityId, TestUsername, NewShoppingCartActivityQuantity);

            var actualQuantity = this.DbContext.ShoppingCartActivities.Find(TestShoppingCartActivityId).Quantity;
            Assert.Equal(NewShoppingCartActivityQuantity, actualQuantity);
        }

        [Fact]
        public void EditGuestShoppingCartActivityThrowsArgumentExceptionIfZeroOrNegativeQuantity()
        {
            var exception = Assert.Throws<ArgumentException>(() => this.ShoppingCartsServiceMock.EditGuestShoppingCartActivity(TestShoppingCartActivityId, null, -1));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public void EditGuestShoppingCartActivityThrowsNullReferenceExceptionIfShoppingCartActivityNotFound()
        {
            var cart = new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = TestShoppingCartActivityId,
                    ActivityId = TestShoppingCartId,
                    ActivityName = TestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = TestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
            };

            var exception = Assert.Throws<NullReferenceException>(() => this.ShoppingCartsServiceMock.EditGuestShoppingCartActivity(SecondTestShoppingCartActivityId,
                cart, TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceGuestShoppingCartActivityId, SecondTestActivityId), exception.Message);
        }

        [Fact]
        public void EditGuestShoppingCartActivityThrowsNullReferenceExceptionIfActivityNotFound()
        {
            var cart = new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = TestShoppingCartActivityId,
                    ActivityId = TestShoppingCartId,
                    ActivityName = TestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = TestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
            };

            var exception = Assert.Throws<NullReferenceException>(() => this.ShoppingCartsServiceMock.EditGuestShoppingCartActivity(TestShoppingCartActivityId,
                cart, TestShoppingCartActivityQuantity));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, TestShoppingCartId), exception.Message);
        }

        [Fact]
        public void EditGuestShoppingCartActivityEditsShoppingCartActivity()
        {
            this.GetAllShoppingCartActivitiesSetDbContext().GetAwaiter().GetResult();
            var cart = this.GetAllShoppingCartActivitiesExpectedResult();

            cart = this.ShoppingCartsServiceMock.EditGuestShoppingCartActivity(TestShoppingCartActivityId,
                 cart, NewShoppingCartActivityQuantity);

            var actualQuantity = cart.FirstOrDefault(c => c.Id == TestShoppingCartActivityId)?.Quantity;
            Assert.Equal(NewShoppingCartActivityQuantity, actualQuantity);
        }

        private ShoppingCartActivityViewModel[] GetAllShoppingCartActivitiesExpectedResult()
        {
            return new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = TestShoppingCartActivityId,
                    ActivityId = TestShoppingCartId,
                    ActivityName = TestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = TestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
                new ShoppingCartActivityViewModel
                {
                    Id = SecondTestShoppingCartActivityId,
                    ActivityId = SecondTestActivityId,
                    ActivityName = SecondTestActivityName,
                    ActivityLocationName = TestLocationName,
                    ShoppingCartUserId = testUserId,
                    ActivityPrice = SecondTestActivityPrice,
                    Quantity = TestShoppingCartActivityQuantity,
                },
            };
        }

        private async Task GetAllShoppingCartActivitiesSetDbContext()
        {
            await this.AddTestingUserWithShoppingCartToDb();
            await this.AddTestingLocationToDb();
            await this.AddTestingActivityToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                LocationId = TestLocationId,
                Price = SecondTestActivityPrice,
            });
            await this.DbContext.SaveChangesAsync();

            this.DbContext.ShoppingCartActivities.Add(new ShoppingCartActivity
            {
                Id = TestShoppingCartActivityId,
                ShoppingCartId = TestShoppingCartId,
                ActivityId = TestActivityId,
                Quantity = TestShoppingCartActivityQuantity
            });
            this.DbContext.ShoppingCartActivities.Add(new ShoppingCartActivity
            {
                Id = SecondTestShoppingCartActivityId,
                ShoppingCartId = TestShoppingCartId,
                ActivityId = SecondTestActivityId,
                Quantity = TestShoppingCartActivityQuantity
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingLocationToDb()
        {
            this.DbContext.Locations.Add(new Location
            {
                Id = TestLocationId,
                Name = TestLocationName,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingActivityToDb()
        {
            this.DbContext.Activities.Add(new Activity
            {
                Id = TestActivityId,
                Name = TestActivityName,
                LocationId = TestLocationId,
                Price = TestActivityPrice,
                Date = testDate,
                ImageUrl = TestImageUrl,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingUserWithShoppingCartToDb()
        {
            this.DbContext.Users.Add(new ApplicationUser
            {
                Id = this.testUserId,
                UserName = TestUsername,
                ShoppingCart = new ShoppingCart
                {
                    Id = TestShoppingCartId,
                    UserId = testUserId,
                }
            });
            await this.DbContext.SaveChangesAsync();
        }
    }
}
