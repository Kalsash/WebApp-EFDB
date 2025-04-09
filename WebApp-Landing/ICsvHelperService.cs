namespace WebApp_Landing
{
    public interface ICsvHelperService
    {
        Task SaveRecordAsync(ContactRecord record);
    }
}
