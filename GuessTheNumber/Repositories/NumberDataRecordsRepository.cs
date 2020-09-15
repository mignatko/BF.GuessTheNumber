using System;
using System.Threading;
using System.Threading.Tasks;
using GuessTheNumber.Models;
using Microsoft.EntityFrameworkCore;

namespace GuessTheNumber.Repositories
{
    public class NumberDataRecordsRepository : INumberDataRecordsRepository
    {
        private ApplicationDbContext _context;
        
        public NumberDataRecordsRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task SaveGuessNumberAndIncreaceAttemptsCount(NumbersData currentRecord, int guessNumber, CancellationToken cancellationToken)
        {
            currentRecord.UsedAttempts += 1;
            
            currentRecord.GuessedNumbers.Add(new GuessedNumber
            {
                NumbersDataId = currentRecord.Id,
                Number = guessNumber,
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<NumbersData> GetNumberDataRecordById(Guid numberDataRecord, CancellationToken cancellationToken)
        {
            return await _context.NumbersDataRecords
                .Include(ndr => ndr.GuessedNumbers)
                .FirstOrDefaultAsync(ndr => ndr.Id == numberDataRecord, cancellationToken);
        }

        public async Task<Guid> InsertNewDataRecord(NumbersData numberDataRecord, CancellationToken cancellationToken)
        {
            await _context.NumbersDataRecords.AddAsync(numberDataRecord, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return numberDataRecord.Id;
        }
    }
}