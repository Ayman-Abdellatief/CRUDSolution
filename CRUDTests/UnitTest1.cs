namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            MyMath myMath = new MyMath();
            int a = 5;
            int b = 10;
            int expected = 15;
            //Act
          int actual =  myMath.Add(a, b);

            //Assert

            Assert.Equal(expected, actual);
        }
    }
}