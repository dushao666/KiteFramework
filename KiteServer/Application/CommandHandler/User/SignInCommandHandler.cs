using Domain.UserInfo;
using Repository.Repositories;

namespace Application.CommandHandler;

public class SignInCommandHandler : IRequestHandler<SignInCommand, LoginUserDto>
{
    private readonly IConfiguration _configuration;
    private readonly IDingTalkService _companyService;
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;

    public SignInCommandHandler(IConfiguration configuration, IDingTalkService companyService,
        ISugarUnitOfWork<DBContext> unitOfWork)
    {
        _configuration = configuration;
        _companyService = companyService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginUserDto> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var model = new LoginUserDto();
        switch (command.Type)
        {
            case LoginType.Password:
                using (var context = _unitOfWork.CreateContext(false))
                {
                    // var password = EncryptionHelper.Sha512(command.Password);
                    var password = command.Password;
                    var user = await context.Users.GetFirstAsync(x =>
                        x.Name == command.Login && x.PassWord == password);
                    if (user is null) throw new UserFriendlyException("用户不存在");
                    model.UserId = user.Id.ToString();
                    model.UserName = user.NickName;
                    model.LoginName = command.Login;
                    return model;
                }
            case LoginType.DingTalk:
                var authResponse =
                    await _companyService.GetUserInfo(_configuration["DingTalk:AgentId"], command.DingTalkAuthCode);
                if (authResponse is null || authResponse.Result is null)
                    throw new UserFriendlyException($"授权失败:{authResponse.Msg}");
                using (var context = _unitOfWork.CreateContext())
                {
                    var userInfo = await context.Users.GetFirstAsync(x => x.DingUserId == authResponse.Result.OpenId);
                    if (userInfo is not null)
                    {
                        model.UserId = userInfo.DingUserId;
                        model.LoginName = userInfo.Name;
                        model.UserName = userInfo.NickName;
                    }
                    else
                    {
                        userInfo = new UserInfo(authResponse.Result.UserName, authResponse.Result.UserName, "",
                            authResponse.Result.OpenId);
                        await context.Users.InsertAsync(userInfo);
                        model.UserId = userInfo.DingUserId;
                        model.LoginName = userInfo.Name;
                        model.UserName = userInfo.NickName;
                        context.Commit();
                    }

                    return model;
                }
            default:
                return null;
        }
    }
}