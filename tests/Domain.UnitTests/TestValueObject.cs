namespace Domain.UnitTests
{
    public class ValueObjectTests
    {
        [Fact]
        public void GetAtomicValues_WhenCalledOnNonInstantiatedObject_ReturnsEmptyCollection()
        {
            // Arrange
            var valueObject = new NonInstantiatedValueObject();

            // Act
            var atomicValues = valueObject.GetAtomicValues();

            // Assert
            Assert.Empty(atomicValues);
        }

        [Fact]
        public void GetAtomicValues_WhenCalledOnValueObjectWithNoProperties_ReturnsEmptyCollection()
        {
            // Arrange
            var valueObject = new EmptyValueObject();

            // Act
            var atomicValues = valueObject.GetAtomicValues();

            // Assert
            Assert.Empty(atomicValues);
        }

        [Fact]
        public void GetAtomicValues_WhenCalledOnValueObjectWithOneProperty_ReturnsCollectionWithOneElement()
        {
            // Arrange
            var valueObject = new SinglePropertyValueObject("test");

            // Act
            var atomicValues = valueObject.GetAtomicValues();

            // Assert
            Assert.Single(atomicValues);
            Assert.Equal("test", atomicValues.First());
        }

        [Fact]
        public void GetAtomicValues_WhenCalledOnValueObjectWithMultipleProperties_ReturnsCollectionWithAllElements()
        {
            // Arrange
            var valueObject = new MultiplePropertiesValueObject("test", 123);

            // Act
            var atomicValues = valueObject.GetAtomicValues();

            // Assert
            Assert.Equal(2, atomicValues.Count());
            Assert.Contains("test", atomicValues);
            Assert.Contains(123, atomicValues);
        }

        private class NonInstantiatedValueObject : ValueObject
        {
            public override IEnumerable<object> GetAtomicValues()
            {
                return [];
            }
        }

        private class EmptyValueObject : ValueObject
        {
            public override IEnumerable<object> GetAtomicValues()
            {
                return [];
            }
        }

        private class SinglePropertyValueObject(string property) : ValueObject
        {
            public string Property { get; } = property;

            public override IEnumerable<object> GetAtomicValues()
            {
                yield return Property;
            }
        }

        private class MultiplePropertiesValueObject(string property1, int property2) : ValueObject
        {
            public string Property1 { get; } = property1;
            public int Property2 { get; } = property2;

            public override IEnumerable<object> GetAtomicValues()
            {
                yield return Property1;
                yield return Property2;
            }
        }
    }
}
