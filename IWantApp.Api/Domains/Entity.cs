using Flunt.Notifications;

namespace IWantApp.Api.Domains;

public abstract class Entity : Notifiable<Notification>
{
    public Entity()
    {
        this.Id = new Guid();
    }
    
    public Guid Id { get; set; }   
    public string? Name { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}