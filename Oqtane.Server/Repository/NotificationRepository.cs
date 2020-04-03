﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Repository.Context;
using Oqtane.Repository.Interfaces;

namespace Oqtane.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private TenantDBContext _db;

        public NotificationRepository(TenantDBContext context)
        {
            _db = context;
        }
            
        public IEnumerable<Notification> GetNotifications(int siteId, int fromUserId, int toUserId)
        {
            if (toUserId == -1 && fromUserId == -1)
            {
                return _db.Notification
                    .Where(item => item.SiteId == siteId)
                    .Where(item => item.IsDelivered == false)
                    .Include(item => item.FromUser)
                    .Include(item => item.ToUser)
                    .ToList();
            }

            return _db.Notification
                .Where(item => item.SiteId == siteId)
                .Where(item => item.ToUserId == toUserId || toUserId == -1)
                .Where(item => item.FromUserId == fromUserId || fromUserId == -1)
                .Include(item => item.FromUser)
                .Include(item => item.ToUser)
                .ToList();
        }

        public Notification AddNotification(Notification notification)
        {
            _db.Notification.Add(notification);
            _db.SaveChanges();
            return notification;
        }

        public Notification UpdateNotification(Notification notification)
        {
            _db.Entry(notification).State = EntityState.Modified;
            _db.SaveChanges();
            return notification;
        }

        public Notification GetNotification(int notificationId)
        {
            return _db.Notification.Find(notificationId);
        }

        public void DeleteNotification(int notificationId)
        {
            Notification notification = _db.Notification.Find(notificationId);
            _db.Notification.Remove(notification);
            _db.SaveChanges();
        }
    }

}
