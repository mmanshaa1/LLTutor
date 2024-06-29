using App.Core.Specifications;

namespace App.APIs.Controllers
{
    [Authorize]
    public class ChatsController : BaseAPIController
    {
        private readonly UserManager<Account> accountManager;

        public ChatsController(IUnitOfWork unitOfWork, UserManager<Account> accountManager_) : base(unitOfWork)
        {
            accountManager = accountManager_;
        }

        #region Get All Chats for a user about specific course
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetChats(int courseId)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            Account? loggedInUser = await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(username);
            if (loggedInUser is null)
                return NotFound(new ApiResponse(404, "user not found!"));

            var course = await unitOfWork.Repository<Course>().GetByIdAsync(courseId);
            if (course is null)
                return NotFound(new ApiResponse(404, "course not found!"));

            var spec = new BaseSpecifications<ChatHistory>(c => c.AccountId == loggedInUser.Id && c.CourseId == courseId);
            var chats = await unitOfWork.Repository<ChatHistory>().GetAllWithSpecAsync(spec);

            var chatsDto = chats.Select(chat => new
            {
                historyId = chat.Id,
                chat.Title
            });

            return Ok(chatsDto);
        }
        #endregion

        #region Get Chat history Data
        [HttpGet("history/{historyId}")]
        public async Task<IActionResult> GetChatHistory(int historyId)
        {
            var history = await unitOfWork.Repository<ChatHistory>().GetByIdAsync(historyId);
            if (history is null)
                return NotFound(new ApiResponse(404, "history not found!"));

            return Ok(new { status = true, history = history.History.Replace("\\", "") });
        }
        #endregion

        #region Open New Chat
        [HttpPost]
        public async Task<IActionResult> OpenNewChat(OpenChatDto new_chat)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            Account? loggedInUser = await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(username);
            if (loggedInUser is null)
                return NotFound(new ApiResponse(404, "user not found!"));

            var course = await unitOfWork.Repository<Course>().GetByIdAsync(new_chat.CourseId);
            if (course is null)
                return NotFound(new ApiResponse(404, "course not found!"));

            var chat = new ChatHistory
            {
                AccountId = loggedInUser.Id,
                CourseId = course.Id,
                Title = new_chat.Title
            };

            await unitOfWork.Repository<ChatHistory>().AddAsync(chat);
            await unitOfWork.CompleteAsync();
            return Ok(new { historyId = chat.Id });
        }

        #endregion

        #region Send Message
        [HttpPost("send")]
        public async Task<IActionResult> OpenNewChat(ChatMessageDto messageDto)
        {
            var history = await unitOfWork.Repository<ChatHistory>().GetByIdAsync(messageDto.HistoryId);
            if (history is null)
                return NotFound(new ApiResponse(404, "history not found!"));

            //unitOfWork.Repository<ChatHistory>().Delete(history);
            //await unitOfWork.CompleteAsync();
            history.History = ChatHistoryHelpers.AddMessage(messageDto.message, "user", history.History);

            history.History = ChatHistoryHelpers.SendMessage(history.History);

            await unitOfWork.CompleteAsync();
            return Ok(new { status = true, history = history.History.Replace("\\", "") });
        }
        #endregion

        #region Delete Chat History

        [HttpDelete("{historyId}")]
        public async Task<IActionResult> DeleteChatHistory(int historyId)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            Account? loggedInUser = await accountManager.FindUserByIdOrUserNameOrEmailOrPhoneAsync(username);
            if (loggedInUser is null)
                return NotFound(new ApiResponse(404, "user not found!"));

            var history = await unitOfWork.Repository<ChatHistory>().GetByIdAsync(historyId);
            if (history is null)
                return NotFound(new ApiResponse(404, "history not found!"));

            if (history.AccountId != loggedInUser.Id)
                return Unauthorized(new ApiResponse(401, "Unauthorized!"));

            unitOfWork.Repository<ChatHistory>().Delete(history);

            await unitOfWork.CompleteAsync();
            return Ok(new { status = true });
        }

        #endregion
    }
}
