using System;
using System.Threading;
using System.Threading.Tasks;
using GuessTheNumber.Models;

namespace GuessTheNumber.Repositories
{
    public interface INumberDataRecordsRepository
    {
        public Task SaveGuessNumberAndIncreaceAttemptsCount(NumbersData currentRecord, int guessNumber, CancellationToken cancellationToken);
        public Task<NumbersData> GetNumberDataRecordById(Guid numberDataRecord, CancellationToken cancellationToken);
        public Task<Guid> InsertNewDataRecord(NumbersData numberDataRecord, CancellationToken cancellationToken);
    }
}