using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public void CheckInItem(int id)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id);

            _context.Update(item);

            RemoveExistingCheckouts(id);
            CloseExistingCheckoutHistory(id, now);
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(id, currentHolds);
                return;
            }

            UpdateAssetStatus(id, "Available");
            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int id, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds
                .HoldPlaced).FirstOrDefault();
            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();
        }

        public void CheckoutItem(int id, int libraryCardId)
        {
            if (IsCheckedOut(id))
            {
                return;
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id);

            UpdateAssetStatus(id, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }
        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public Checkout Get(int id)
        {
            return GetAll().FirstOrDefault(checkout => checkout.Id == id);
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentHoldPatron(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null)
            {
                return "Not checked out";
            };

            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);
            return $"{patron.FirstName} {patron.LastName}";
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }

        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == id)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public int GetNumberOfCopies(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsCheckedOut(int id)
        {
            return _context.Checkouts
                 .Where(co => co.LibraryAsset.Id == id)
                 .Any();
        }

        public void MarkFound(int id)
        {
            var now = DateTime.Now;

            UpdateAssetStatus(id, "Available");
            RemoveExistingCheckouts(id);
            CloseExistingCheckoutHistory(id, now);

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int id, string s)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id);

            _context.Update(item);
            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == s);
        }   

        private void RemoveExistingCheckouts(int id)
        {
            var checkout = _context.Checkouts
                .FirstOrDefault(co => co.LibraryAsset.Id == id);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        private void CloseExistingCheckoutHistory(int id, DateTime now)
        {
            var history = _context.CheckoutHistories
                .FirstOrDefault(h => h.LibraryAsset.Id == id
                && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        public void MarkLost(int id)
        {
            UpdateAssetStatus(id, "Lost");
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == libraryCardId);

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }
    }
}
