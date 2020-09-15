using System;
using System.Threading;
using System.Threading.Tasks;
using GuessTheNumber.Configuration;
using GuessTheNumber.Models;
using GuessTheNumber.Models.DTO;
using GuessTheNumber.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GuessTheNumber.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class NumbersController : ControllerBase
    {

        private INumberDataRecordsRepository _numbersRepository;
        private NumbersSettings _numbersSettings;
        
        public NumbersController(ApplicationDbContext context, IOptions<NumbersSettings> servicesSettings, INumberDataRecordsRepository numbersRepository)
        {
            _numbersRepository = numbersRepository;
            _numbersSettings = servicesSettings?.Value 
                               ?? throw new ArgumentNullException(nameof(servicesSettings));
        }

        /// <summary>
        /// Create a new Number Record
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ID of newly created NumberRecord</returns>
        [HttpPost]
        public async Task<IActionResult> InsertNewNumberRecord(CancellationToken cancellationToken)
        {
            try {
                var newNumberRecord = new NumbersData
                {
                    Id = Guid.NewGuid(),
                    PredefinedNumber = GetRandomNumberFromPredefinedRange(),
                };
                return Ok(await _numbersRepository.InsertNewDataRecord(newNumberRecord, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get <see cref="NumbersData"/> record with guessed numbers
        /// </summary>
        /// <param name="numberRecordId">ID of NumberData record</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Returns <see cref="NumbersData"/> object</returns>
        [HttpGet("{numberRecordId}")]
        public async Task<IActionResult> GetRecordById(Guid numberRecordId, CancellationToken cancellationToken)
        {
            try
            {
                var record = await _numbersRepository.GetNumberDataRecordById(numberRecordId, cancellationToken);
                
                if (record != null)
                    return Ok(record);
                
                throw new ArgumentOutOfRangeException(nameof(numberRecordId), "NumberData record with provided Id wasn't fond in the DB");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        /// <summary>
        /// Handle number guess request using <see cref="GuessNumberDTO"/> model
        /// </summary>
        /// <param name="requestModel"><see cref="GuessNumberDTO"/> model</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Boolean result of operation</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case if request Model is not containing valid values</exception>
        [HttpPut("guess")]
        public async Task<IActionResult> GuessTheNumberForCurrentRecord([FromBody]GuessNumberDTO requestModel, CancellationToken cancellationToken)
        {
            try
            {
                CheckGuessedNumberIsInsideTheRange(requestModel.GuessNumber);

                var currentRecord = await _numbersRepository.GetNumberDataRecordById(requestModel.NumberRecordId, cancellationToken);

                ValidateNumberDataRecord(currentRecord);

                await _numbersRepository.SaveGuessNumberAndIncreaceAttemptsCount(currentRecord, requestModel.GuessNumber, cancellationToken);
            
                if (currentRecord.PredefinedNumber == requestModel.GuessNumber)
                    return Ok(true);
            
                return Ok(false);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Returns predefined numbers range
        /// </summary>
        /// <returns>Returns <see cref="NumbersRangeDTO"/></returns>
        [HttpGet("get_range")]
        public IActionResult GetPredefinedNumbersRange()
        { 
            return Ok(new NumbersRangeDTO
            {
                RangeStart = _numbersSettings.StartRangeNumber, 
                RangeEnd = _numbersSettings.EndRangeNumber
            });
        }
        
        private void ValidateNumberDataRecord(NumbersData currentRecord)
        {
            if (currentRecord == null)
                throw new ArgumentOutOfRangeException(nameof(GuessNumberDTO.NumberRecordId));
            
            if(currentRecord.UsedAttempts == _numbersSettings.MaxAttemptsCount)
                throw new InvalidOperationException("All guess attempts are used for current record"); 
        }
        
        private void CheckGuessedNumberIsInsideTheRange(int guessNumber)
        {
            if (guessNumber < _numbersSettings.StartRangeNumber || guessNumber > _numbersSettings.EndRangeNumber)
                throw new ArgumentOutOfRangeException(nameof(GuessNumberDTO.GuessNumber));
        }

        private int GetRandomNumberFromPredefinedRange()
        {
            var randomGenerator = new Random();
            return randomGenerator.Next(_numbersSettings.StartRangeNumber, _numbersSettings.EndRangeNumber);
        }
    }
}