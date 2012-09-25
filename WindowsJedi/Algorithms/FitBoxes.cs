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
		public int AlgFillByStripes (int bin_width, Rectangle[] rects) {
			// Sort by height.
			var best_rects = (Rectangle[])rects.Clone();
			Array.Sort(best_rects, new HeightComparer());

			// Make variables to track and record the best solution.
			var is_positioned = new bool[best_rects.Length];
			var num_unpositioned = best_rects.Length;

			// Fill by stripes.
			int max_y = 0;
			for (int i = 0; i <= rects.Length - 1; i++) {
				// See if this rectangle is positioned.
				if (!is_positioned[i]) {
					// Start a new stripe.
					num_unpositioned -= 1;
					is_positioned[i] = true;
					best_rects[i].X = 0;
					best_rects[i].Y = max_y;

					FillBoundedArea(
						best_rects[i].Width, bin_width, max_y,
						max_y + best_rects[i].Height,
						ref num_unpositioned, ref best_rects, ref is_positioned);
					
					if (num_unpositioned == 0) break;
					max_y += best_rects[i].Height;
				}
			}

			// Save the best solution.
			Array.Copy(best_rects, rects, rects.Length);
			return max_y;
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
			ref int num_unpositioned, ref Rectangle[] rects, ref bool[] is_positioned) {
			// See if every rectangle has been positioned.
			if (num_unpositioned <= 0) return;

			// Save a copy of the solution so far.
			int best_num_unpositioned = num_unpositioned;
			var best_rects = (Rectangle[])rects.Clone();
			var best_is_positioned = (bool[])is_positioned.Clone();

			// Currently we have no solution for this area.
			double best_density = 0;

			// Some rectangles have not been positioned.
			// Loop through the available rectangles.
			for (int i = 0; i <= rects.Length - 1; i++) {
				// See if this rectangle is not position and will fit.
				if ((!is_positioned[i]) &&
					(rects[i].Width <= xmax - xmin) &&
					(rects[i].Height <= ymax - ymin)) {
					// It will fit. Try it.
					// **************************************************
					// Divide the remaining area horizontally.
					int test1_num_unpositioned = num_unpositioned - 1;
					var test1_rects = (Rectangle[])rects.Clone();
					var test1_is_positioned = (bool[])is_positioned.Clone();
					test1_rects[i].X = xmin;
					test1_rects[i].Y = ymin;
					test1_is_positioned[i] = true;

					// Fill the area on the right.
					FillBoundedArea(xmin + rects[i].Width, xmax, ymin, ymin + rects[i].Height,
						ref test1_num_unpositioned, ref test1_rects, ref test1_is_positioned);
					// Fill the area on the bottom.
					FillBoundedArea(xmin, xmax, ymin + rects[i].Height, ymax,
						ref test1_num_unpositioned, ref test1_rects, ref test1_is_positioned);

					// Learn about the test solution.
					double test1_density =
						SolutionDensity(
							xmin + rects[i].Width, xmax, ymin, ymin + rects[i].Height,
							xmin, xmax, ymin + rects[i].Height, ymax,
							test1_rects, test1_is_positioned);

					// See if this is better than the current best solution.
					if (test1_density >= best_density) {
						// The test is better. Save it.
						best_density = test1_density;
						best_rects = test1_rects;
						best_is_positioned = test1_is_positioned;
						best_num_unpositioned = test1_num_unpositioned;
					}

					// **************************************************
					// Divide the remaining area vertically.
					int test2_num_unpositioned = num_unpositioned - 1;
					var test2_rects = (Rectangle[])rects.Clone();
					var test2_is_positioned = (bool[])is_positioned.Clone();
					test2_rects[i].X = xmin;
					test2_rects[i].Y = ymin;
					test2_is_positioned[i] = true;

					// Fill the area on the right.
					FillBoundedArea(xmin + rects[i].Width, xmax, ymin, ymax,
						ref test2_num_unpositioned, ref test2_rects, ref test2_is_positioned);
					// Fill the area on the bottom.
					FillBoundedArea(xmin, xmin + rects[i].Width, ymin + rects[i].Height, ymax,
						ref test2_num_unpositioned, ref test2_rects, ref test2_is_positioned);

					// Learn about the test solution.
					double test2_density =
						SolutionDensity(
							xmin + rects[i].Width, xmax, ymin, ymax,
							xmin, xmin + rects[i].Width, ymin + rects[i].Height, ymax,
							test2_rects, test2_is_positioned);

					// See if this is better than the current best solution.
					if (test2_density >= best_density) {
						// The test is better. Save it.
						best_density = test2_density;
						best_rects = test2_rects;
						best_is_positioned = test2_is_positioned;
						best_num_unpositioned = test2_num_unpositioned;
					}
				} // End trying this rectangle.
			} // End looping through the rectangles.

			// Return the best solution we found.
			is_positioned = best_is_positioned;
			num_unpositioned = best_num_unpositioned;
			rects = best_rects;
		}

		// Find the density of the rectangles in the given areas for this solution.
		private double SolutionDensity (
			int xmin1, int xmax1, int ymin1, int ymax1,
			int xmin2, int xmax2, int ymin2, int ymax2,
			Rectangle[] rects, bool[] is_positioned) {
			var rect1 = new Rectangle(xmin1, ymin1, xmax1 - xmin1, ymax1 - ymin1);
			var rect2 = new Rectangle(xmin2, ymin2, xmax2 - xmin2, ymax2 - ymin2);
			int area_covered = 0;
			for (int i = 0; i <= rects.Length - 1; i++) {
				if (is_positioned[i] &&
					(rects[i].IntersectsWith(rect1) ||
					 rects[i].IntersectsWith(rect2))) {
					area_covered += rects[i].Width * rects[i].Height;
				}
			}

			double denom = rect1.Width * rect1.Height + rect2.Width * rect2.Height;
			if (Math.Abs(denom) < 0.001) return 0;

			return area_covered / denom;
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
