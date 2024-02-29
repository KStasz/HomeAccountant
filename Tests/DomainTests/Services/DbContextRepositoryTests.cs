using Xunit;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = Xunit.Assert;
using DomainTests.Mock;
using Moq;

namespace Domain.Services.Tests
{
    public class DbContextRepositoryTests
    {
        private readonly IRepository<InMemoryDatabaseService, SampleModelMock> _dbRepository;
        private readonly DbContextRepository<InMemoryDatabaseService, ReferencedSampleModelMock> _dbReferencedRepository;

        public DbContextRepositoryTests()
        {
            var dbContext = new InMemoryDatabaseService();
            dbContext.Database.EnsureCreated();

            _dbRepository = new DbContextRepository<InMemoryDatabaseService, SampleModelMock>(
                dbContext);
            _dbReferencedRepository = new DbContextRepository<InMemoryDatabaseService, ReferencedSampleModelMock>(
                dbContext);
        }

        [Fact()]
        public async Task Add_ShouldAddObjectToDatabase()
        {
            var expected = new SampleModelMock();
            var result = _dbRepository.Add(expected)?.Entity;
            var affectedRecords = await _dbRepository.SaveChangesAsync();
            var actual = _dbRepository.GetAll(x => true).First();

            Assert.Equal(expected, actual);
            Assert.Equal(1, affectedRecords);
            Assert.Equal(expected, result);
        }

        [Fact()]
        public async Task Add_ShouldNotAddNullObject()
        {
            var result = _dbRepository.Add(null!);
            var affectedRecords = await _dbRepository.SaveChangesAsync();

            Assert.Equal(0, affectedRecords);
            Assert.Null(result);
        }

        [Fact()]
        public async Task Get_ShouldReturnSpecificObject()
        {
            var expectedModel = new SampleModelMock()
            {
                Id = 1,
                Name = It.IsAny<string>(),
                ReferencedSampleModelMockId = It.IsAny<int>(),
                ReferencedSampleModelMock = It.IsAny<ReferencedSampleModelMock>()
            };

            _dbRepository.Add(expectedModel);
            await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.Get(x => x.Id == 1);

            Assert.NotNull(actual);
            Assert.Equal(expectedModel, actual);
        }

        [Fact()]
        public async Task Get_ShouldNotReturnSepecificObjectIfAddingFailed()
        {
            SampleModelMock? expectedModel = null;

            _dbRepository.Add(expectedModel!);
            var affectedRecords = await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.Get(x => x.Id == 1);
            var collection = _dbRepository.GetAll(x => true).ToList();

            Assert.Equal(0, affectedRecords);
            Assert.Null(actual);
            Assert.Empty(collection);
        }

        [Fact()]
        public async Task Get_ShouldMapReferencedObjectOfSpecificObject()
        {
            var expectedObject = new SampleModelMock()
            {
                Name = It.IsAny<string>()
            };

            _dbRepository.Add(expectedObject);
            await _dbRepository.SaveChangesAsync();

            var expectedReferencedObject = new ReferencedSampleModelMock()
            {
                Name = It.IsAny<string>(),
                Description = It.IsAny<string>()
            };

            _dbReferencedRepository.Add(expectedReferencedObject);
            await _dbReferencedRepository.SaveChangesAsync();


            expectedObject.ReferencedSampleModelMockId = expectedReferencedObject.Id;
            expectedObject.ReferencedSampleModelMock = expectedReferencedObject;

            _dbRepository.Update(expectedObject);
            await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.Get(x => x.Id == 1, y => y.ReferencedSampleModelMock!);

            Assert.NotNull(actual);
            Assert.NotNull(actual?.ReferencedSampleModelMock);
            Assert.NotNull(actual?.ReferencedSampleModelMockId);
        }

        [Fact()]
        public async Task GetAll_ShouldReturnAllObjectsThatMeetTheCondition()
        {
            var expectedObjects = new List<SampleModelMock>()
            {
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                }
            };

            expectedObjects.ForEach(x => _dbRepository.Add(x));
            await _dbRepository.SaveChangesAsync();

            var actualObjects = _dbRepository.GetAll(x => true);

            Assert.NotNull(actualObjects);
            Assert.NotEmpty(actualObjects);
            Assert.Equal(expectedObjects.Count, actualObjects.Count());
        }

        [Fact()]
        public async Task GetAll_ShouldReturnAllObjectsThatMeetConditionWithReferencedObjects()
        {
            var expectedObjects = new List<SampleModelMock>()
            {
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                }
            };

            var expectedReferencedObjects = new List<ReferencedSampleModelMock>()
            {
                new ReferencedSampleModelMock()
                {
                    Name = It.IsAny<string>(),
                    Description = It.IsAny<string>()
                },
                new ReferencedSampleModelMock()
                {
                    Name = It.IsAny<string>(),
                    Description = It.IsAny<string>()
                },
                new ReferencedSampleModelMock()
                {
                    Name = It.IsAny<string>(),
                    Description = It.IsAny<string>()
                }
            };

            expectedObjects.ForEach(x => _dbRepository.Add(x));
            expectedReferencedObjects.ForEach(x => _dbReferencedRepository.Add(x));

            await _dbRepository.SaveChangesAsync();
            await _dbReferencedRepository.SaveChangesAsync();

            expectedObjects[0].ReferencedSampleModelMockId = expectedReferencedObjects[0].Id;
            expectedObjects[0].ReferencedSampleModelMock = expectedReferencedObjects[0];

            expectedObjects[1].ReferencedSampleModelMockId = expectedReferencedObjects[1].Id;
            expectedObjects[1].ReferencedSampleModelMock = expectedReferencedObjects[1];

            expectedObjects[2].ReferencedSampleModelMockId = expectedReferencedObjects[2].Id;
            expectedObjects[2].ReferencedSampleModelMock = expectedReferencedObjects[2];

            expectedObjects.ForEach(x => _dbRepository.Update(x));
            await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.GetAll(x => true, x => x.ReferencedSampleModelMock!).ToList();

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.All(x => x.ReferencedSampleModelMock is not null));
            Assert.True(actual.All(x => x.ReferencedSampleModelMockId is not null));
        }

        [Fact()]
        public async Task Remove_ShouldRemoveSpecificObjects()
        {
            var sampleObjects = new List<SampleModelMock>()
            {
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                }
            };

            sampleObjects.ForEach(x => _dbRepository.Add(x));
            await _dbRepository.SaveChangesAsync();

            _dbRepository.Remove(sampleObjects[2]);
            await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.GetAll(x => true);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(2, actual.Count());
        }

        [Fact()]
        public async Task RemoveMany_ShouldRemoveSpecificObjects()
        {
            var sampleObjects = new List<SampleModelMock>()
            {
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                }
            };

            sampleObjects.ForEach(x => _dbRepository.Add(x));
            await _dbRepository.SaveChangesAsync();

            _dbRepository.RemoveMany(sampleObjects.Take(2));
            await _dbRepository.SaveChangesAsync();

            var actual = _dbRepository.GetAll(x => true);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Single(actual);
        }

        [Fact()]
        public async Task SaveChangesAsync_ShouldReturnOneAfterSavingOneObject()
        {
            var specificObjects = new SampleModelMock()
            {
                Name = It.IsAny<string>()
            };

            _dbRepository.Add(specificObjects);
            var result = await _dbRepository.SaveChangesAsync();

            Assert.Equal(1, result);
        }

        [Fact()]
        public async Task SaveChangesAsync_ShouldReturnActualAmountOfSavedObjects()
        {
            var sampleObjects = new List<SampleModelMock>()
            {
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                },
                new SampleModelMock()
                {
                    Name = It.IsAny<string>()
                }
            };

            sampleObjects.ForEach(x => _dbRepository.Add(x));
            var result = await _dbRepository.SaveChangesAsync();

            Assert.Equal(3, result);
        }

        [Fact()]
        public async Task SaveChangesAsync_ShouldReturnZeroIfAddedNothingToDatabase()
        {
            var result = await _dbRepository.SaveChangesAsync();

            Assert.Equal(0, result);
        }

        [Fact()]
        public async Task SaveChangesAsync_ShouldReturnZeroIfTriedToSaveNullObject()
        {
            _dbRepository.Add(null!);
            var result = await _dbRepository.SaveChangesAsync();

            Assert.Equal(0, result);
        }

        [Fact()]
        public async Task Update_ShouldUpdateObject()
        {
            var sampleObject = new SampleModelMock()
            {
                Name = It.IsAny<string>()
            };

            _dbRepository.Add(sampleObject);
            await _dbRepository.SaveChangesAsync();

            Assert.Null(sampleObject.Name);

            var existingObject = _dbRepository.Get(x => x.Id == 1);
            existingObject!.Name = "Test";
            
            _dbRepository.Update(existingObject);
            var result = await _dbRepository.SaveChangesAsync();

            var afterUpdateObject = _dbRepository.Get(x => x.Id == 1);

            Assert.NotNull(afterUpdateObject);
            Assert.NotNull(afterUpdateObject.Name);
            Assert.Equal(1, result);
        }
    }
}