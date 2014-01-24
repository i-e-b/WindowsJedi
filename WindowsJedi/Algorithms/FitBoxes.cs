using System;
using System.Collections;
using System.Drawing;

namespace WindowsJedi.Algorithms {
	/// <summary>
	/// Box fitting algorithms
	/// </summary>
	public class FitBoxes {

		/// <summary>
		/// Fit rectangles into a given width. Total height required is returned
		/// </summary>
		public int AlgFillByStripes (int binWidth, Rectangle[] rects) {
			// Sort by height.
			var bestRects = (Rectangle[])rects.Clone();
			Array.Sort(bestRects, new HeightComparer());

			// Make variables to track and record the best solution.
			var isPositioned = new bool[bestRects.Length];
			var numUnpositioned = bestRects.Length;

			// Fill by stripes.
			int maxY = 0;
			for (int i = 0; i <= rects.Length - 1; i++) {
				// See if this rectangle is positioned.
				if (!isPositioned[i]) {
					// Start a new stripe.
					numUnpositioned -= 1;
					isPositioned[i] = true;
					bestRects[i].X = 0;
					bestRects[i].Y = maxY;

					FillBoundedArea(
						bestRects[i].Width, binWidth, maxY,
						maxY + bestRects[i].Height,
						ref numUnpositioned, ref bestRects, ref isPositioned);
					
					if (numUnpositioned == 0) break;
					maxY += bestRects[i].Height;
				}
			}

			// Save the best solution.
			Array.Copy(bestRects, rects, rects.Length);
			return maxY;
		}

		// Use rectangles to fill the given sub-area.
		// Set the following for the best solution we find:
		//       xmin, xmax, etc.    - Bounds of the rectangle we are trying to fill.
		//       num_unpositioned    - The number of rectangles not yet positioned in this solution.
		//                             Used to control the recursion.
		//       rects()             - All rectangles for the problem, some positioned and others not. 
		//                             Initially this is the partial solution we are working from.
		//                             At end, this is the best solution we could find.
		//       is_positioned()     - Indicates which rectangles are positioned in this solution.
		//       max_y               - The largest Y value for this solution.
		private void FillBoundedArea (
			int xmin, int xmax, int ymin, int ymax,
			ref int numUnpositioned, ref Rectangle[] rects, ref bool[] isPositioned) {
			// See if every rectangle has been positioned.
			if (numUnpositioned <= 0) return;

			// Save a copy of the solution so far.
			int bestNumUnpositioned = numUnpositioned;
			var bestRects = (Rectangle[])rects.Clone();
			var bestIsPositioned = (bool[])isPositioned.Clone();

			// Currently we have no solution for this area.
			double bestDensity = 0;

			// Some rectangles have not been positioned.
			// Loop through the available rectangles.
			for (int i = 0; i <= rects.Length - 1; i++) {
				// See if this rectangle is not position and will fit.
				if ((!isPositioned[i]) &&
					(rects[i].Width <= xmax - xmin) &&
					(rects[i].Height <= ymax - ymin)) {
					// It will fit. Try it.
					// **************************************************
					// Divide the remaining area horizontally.
					int test1NumUnpositioned = numUnpositioned - 1;
					var test1Rects = (Rectangle[])rects.Clone();
					var test1IsPositioned = (bool[])isPositioned.Clone();
					test1Rects[i].X = xmin;
					test1Rects[i].Y = ymin;
					test1IsPositioned[i] = true;

					// Fill the area on the right.
					FillBoundedArea(xmin + rects[i].Width, xmax, ymin, ymin + rects[i].Height,
						ref test1NumUnpositioned, ref test1Rects, ref test1IsPositioned);
					// Fill the area on the bottom.
					FillBoundedArea(xmin, xmax, ymin + rects[i].Height, ymax,
						ref test1NumUnpositioned, ref test1Rects, ref test1IsPositioned);

					// Learn about the test solution.
					double test1Density =
						SolutionDensity(
							xmin + rects[i].Width, xmax, ymin, ymin + rects[i].Height,
							xmin, xmax, ymin + rects[i].Height, ymax,
							test1Rects, test1IsPositioned);

					// See if this is better than the current best solution.
					if (test1Density >= bestDensity) {
						// The test is better. Save it.
						bestDensity = test1Density;
						bestRects = test1Rects;
						bestIsPositioned = test1IsPositioned;
						bestNumUnpositioned = test1NumUnpositioned;
					}

					// **************************************************
					// Divide the remaining area vertically.
					int test2NumUnpositioned = numUnpositioned - 1;
					var test2Rects = (Rectangle[])rects.Clone();
					var test2IsPositioned = (bool[])isPositioned.Clone();
					test2Rects[i].X = xmin;
					test2Rects[i].Y = ymin;
					test2IsPositioned[i] = true;

					// Fill the area on the right.
					FillBoundedArea(xmin + rects[i].Width, xmax, ymin, ymax,
						ref test2NumUnpositioned, ref test2Rects, ref test2IsPositioned);
					// Fill the area on the bottom.
					FillBoundedArea(xmin, xmin + rects[i].Width, ymin + rects[i].Height, ymax,
						ref test2NumUnpositioned, ref test2Rects, ref test2IsPositioned);

					// Learn about the test solution.
					double test2Density =
						SolutionDensity(
							xmin + rects[i].Width, xmax, ymin, ymax,
							xmin, xmin + rects[i].Width, ymin + rects[i].Height, ymax,
							test2Rects, test2IsPositioned);

					// See if this is better than the current best solution.
					if (test2Density >= bestDensity) {
						// The test is better. Save it.
						bestDensity = test2Density;
						bestRects = test2Rects;
						bestIsPositioned = test2IsPositioned;
						bestNumUnpositioned = test2NumUnpositioned;
					}
				} // End trying this rectangle.
			} // End looping through the rectangles.

			// Return the best solution we found.
			isPositioned = bestIsPositioned;
			numUnpositioned = bestNumUnpositioned;
			rects = bestRects;
		}

		// Find the density of the rectangles in the given areas for this solution.
		private double SolutionDensity (
			int xmin1, int xmax1, int ymin1, int ymax1,
			int xmin2, int xmax2, int ymin2, int ymax2,
			Rectangle[] rects, bool[] isPositioned) {
			var rect1 = new Rectangle(xmin1, ymin1, xmax1 - xmin1, ymax1 - ymin1);
			var rect2 = new Rectangle(xmin2, ymin2, xmax2 - xmin2, ymax2 - ymin2);
			int areaCovered = 0;
			for (int i = 0; i <= rects.Length - 1; i++) {
				if (isPositioned[i] &&
					(rects[i].IntersectsWith(rect1) ||
					 rects[i].IntersectsWith(rect2))) {
					areaCovered += rects[i].Width * rects[i].Height;
				}
			}

			double denom = rect1.Width * rect1.Height + rect2.Width * rect2.Height;
			if (Math.Abs(denom) < 0.001) return 0;

			return areaCovered / denom;
		}

		private class HeightComparer : IComparer {
			public int Compare (object x, object y) {
				var xrect = (Rectangle)x;
				var yrect = (Rectangle)y;
				if (xrect.Height < yrect.Height) return 1;
				if (xrect.Height > yrect.Height) return -1;
				if (xrect.Width < yrect.Width) return 1;
				if (xrect.Width > yrect.Width) return -1;
				return 0;
			}
		}
	}
}
