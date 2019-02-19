using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using BlogDotNet.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogDotNet.Services
{
    public class UserSubscriptionsService
    {
        private readonly ApplicationDbContext _context;

        public UserSubscriptionsService(ApplicationDbContext context,
            UsersService usersService)
        {
            _context = context;
        }

        public async Task<Tuple<List<ApplicationUser>, List<ApplicationUser>, int>> GetUserSubscriptions(string userId)
        {
            IQueryable<UserRelation> queryable = _context.UserRelations.Include(ur => ur.Follower)
                .Include(ur => ur.Following)
                .Where(ur => ur.FollowerUserId == userId || ur.FollowingUserId == userId);

            int relationsCount = await queryable.CountAsync();
            List<UserRelation> subscriptions = await queryable.ToListAsync();
            List<ApplicationUser> followers = new List<ApplicationUser>();
            List<ApplicationUser> following = new List<ApplicationUser>();

            subscriptions.ForEach(ur =>
            {
                if (ur.FollowerUserId == userId)
                    following.Add(ur.Following);

                else if (ur.FollowingUserId == userId)
                    followers.Add(ur.Follower);
            });

            return Tuple.Create(following, followers, relationsCount);
        }

        public async Task<UserRelation> GetUserSubscription(string followingId, string followerId)
        {
            return await _context.UserRelations.SingleOrDefaultAsync(ur =>
                ur.FollowingUserId == followingId && ur.FollowerUserId == followerId);
        }

        public async Task<bool> CreateUserRelation(string followingId, string followerId)
        {
            await _context.UserRelations.AddAsync(new UserRelation
            {
                FollowerUserId = followerId,
                FollowingUserId = followingId
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserSubscription(string followingId, string followerId)
        {
            UserRelation ur = await GetUserSubscription(followingId, followerId);
            return await DeleteUserSubscription(ur);
        }

        public async Task<bool> DeleteUserSubscription(UserRelation ur)
        {
            _context.Remove(ur);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}