using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Moq;
using Newtonsoft.Json;
using System.Reflection;
using Xunit;
using Assert = Xunit.Assert;

namespace HomeAccountant.Core.ViewModels.Tests
{
    public class MvvmViewModelTests
    {
        private Mock<ICategoriesService> _categoriesServiceMock;
        private CategoriesViewModel _viewModel;

        public static IEnumerable<object[]> GetParameter_Parameters => new List<object[]>
        {
            new object[] { typeof(int), typeof(int), "1", 1 },
            new object[] { typeof(string), typeof(string), "word", "word" },
            new object[] { typeof(double), typeof(double), "1,2", 1.2 },
            new object[] { typeof(bool), typeof(bool), "false", false },
            new object[] { typeof(bool), typeof(bool), "False", false },
            new object[] { typeof(bool), typeof(bool), "true", true },
            new object[] { typeof(bool), typeof(bool), "True", true },
            new object[] { typeof(int), typeof(int), 1, 1 },
            new object[] { typeof(string), typeof(string), "word", "word" },
            new object[] { typeof(double), typeof(double), 1.2, 1.2 },
            new object[] { typeof(bool), typeof(bool), false, false },
            new object[] { typeof(bool), typeof(bool), true, true },
            new object[] { typeof(List<string>), typeof(List<string>), new List<string>() { string.Empty }, new List<string>() { string.Empty } },
            new object[] { typeof(List<int>), typeof(List<int>), new List<int>() { 0 }, new List<int>() { 0 } },
            new object[] { typeof(int[]), typeof(int[]), new int[] { 0 }, new int[] { 0 } },
            new object[] { typeof(object), typeof(object), new object(), new object() },
            new object[] { typeof(RegisterModel), typeof(RegisterModel), new RegisterModel(), new RegisterModel() }
        };

        public MvvmViewModelTests()
        {
            _categoriesServiceMock = new Mock<ICategoriesService>();
            _viewModel = new CategoriesViewModel(_categoriesServiceMock.Object);
        }

        [Theory()]
        [MemberData(nameof(GetParameter_Parameters))]
        public void GetParameter_Test(Type expectedType, Type actualType, object objectToConvert, object expectedObject)
        {
            MethodInfo? method = _viewModel
                .GetType()
                .GetMethod(nameof(_viewModel.GetParameter))?
                .MakeGenericMethod(actualType);

            var result = method?.Invoke(_viewModel, [objectToConvert]);

            Assert.NotNull(result);
            Assert.Equal(expectedType, result.GetType());
            Assert.Equal(JsonConvert.SerializeObject(expectedObject), JsonConvert.SerializeObject(result));
        }
    }
}