namespace BlogDotNet.Entities
{
    public class UserRelation
    {
        public string FollowingUserId { get; set; }
        public string FollowerUserId { get; set; }

        public ApplicationUser Following { get; set; }
        public ApplicationUser Follower { get; set; }
    }
}