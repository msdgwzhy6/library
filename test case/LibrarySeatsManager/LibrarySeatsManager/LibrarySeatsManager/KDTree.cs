using System;
using System.Collections;

public class SortedLimitedList : ArrayList
{
	private SortedLimitedList ()
	{
	}

	int max;

	public SortedLimitedList (int maxElements)
		: base (maxElements)
	{
		max = maxElements;
	}

	public override int Add (object obj)
	{
		int pos = Count;

		while (pos > 0 && ((IComparable)base[pos-1]).CompareTo (obj) >= 0) {
			if (pos < max) {
				Set (pos, base[pos-1]);
			}
			pos --;
		}

		if (pos < max) {
			Set (pos, obj);
		} else {
			pos = -1;
		}

		return pos;
	}

	internal void Set (int idx, object obj)
	{
		if (idx < Count) {
			base[idx] = obj;
		} else if (idx == Count) {
			base.Add (obj);
		}
	}
}


public class KDTree
{
	IKDTreeDomain dr;

	int splitDim;

	KDTree left;
	KDTree right;


	private KDTree ()
	{
	}

	public class BestEntry : IComparable
	{
		private double dist;
		public double Distance {
			get {
				return (dist);
			}
			set {
				dist = value;
			}
		}

		private int distSq;
		public int DistanceSq {
			get {
				return (distSq);
			}
			set {
				distSq = value;
			}
		}

		IKDTreeDomain neighbour;
		public IKDTreeDomain Neighbour {
			get {
				return (neighbour);
			}
		}

		public static new bool Equals (object obj1, object obj2)
		{
			BestEntry be1 = (BestEntry) obj1;
			BestEntry be2 = (BestEntry) obj2;

			return (be1.Neighbour == be2.Neighbour);
		}

		private BestEntry ()
		{
		}

		internal BestEntry (IKDTreeDomain neighbour, int distSq, bool squared)
		{
			this.neighbour = neighbour;
			this.distSq = distSq;
		}

		internal BestEntry (IKDTreeDomain neighbour, double dist)
		{
			this.neighbour = neighbour;
			this.dist = dist;
		}

		public int CompareTo (object obj)
		{
			BestEntry be = (BestEntry) obj;

			if (distSq < be.distSq)
				return (-1);
			else if (distSq > be.distSq)
				return (1);

			return (0);
		}
	}

	internal class HREntry : IComparable
	{
		double dist;
		internal double Distance {
			get {
				return (dist);
			}
		}

		HyperRectangle rect;
		internal HyperRectangle HR {
			get {
				return (rect);
			}
		}
		IKDTreeDomain pivot;
		internal IKDTreeDomain Pivot {
			get {
				return (pivot);
			}
		}

		KDTree tree;
		internal KDTree Tree {
			get {
				return (tree);
			}
		}

		private HREntry ()
		{
		}

		internal HREntry (HyperRectangle rect, KDTree tree, IKDTreeDomain pivot,
			double dist)
		{
			this.rect = rect;
			this.tree = tree;
			this.pivot = pivot;
			this.dist = dist;
		}

		public int CompareTo (object obj)
		{
			HREntry hre = (HREntry) obj;

			if (dist < hre.dist)
				return (-1);
			else if (dist > hre.dist)
				return (1);

			return (0);
		}
	}

	internal class HyperRectangle : ICloneable
	{
		int[] leftTop;
		int[] rightBottom;
		int dim;

		private HyperRectangle ()
		{
		}

		private HyperRectangle (int dim)
		{
			this.dim = dim;
			leftTop = new int[dim];
			rightBottom = new int[dim];
		}

		public object Clone ()
		{
			HyperRectangle rec = new HyperRectangle (dim);

			for (int n = 0 ; n < dim ; ++n) {
				rec.leftTop[n] = leftTop[n];
				rec.rightBottom[n] = rightBottom[n];
			}

			return (rec);
		}

		static internal HyperRectangle CreateUniverseRectangle (int dim)
		{
			HyperRectangle rec = new HyperRectangle (dim);

			for (int n = 0 ; n < dim ; ++n) {
				rec.leftTop[n] = Int32.MinValue;
				rec.rightBottom[n] = Int32.MaxValue;
			}

			return (rec);
		}

		internal HyperRectangle SplitAt (int splitDim, int splitVal)
		{
			if (leftTop[splitDim] >= splitVal || rightBottom[splitDim] < splitVal)
				throw (new ArgumentException ("SplitAt with splitpoint outside rec"));

			HyperRectangle r2 = (HyperRectangle) this.Clone ();
			rightBottom[splitDim] = splitVal;
			r2.leftTop[splitDim] = splitVal;

			return (r2);
		}

		internal bool IsIn (IKDTreeDomain target)
		{
			if (target.DimensionCount != dim)
				throw (new ArgumentException ("IsIn dimension mismatch"));

			for (int n = 0 ; n < dim ; ++n) {
				int targD = target.GetDimensionElement (n);

				if (targD < leftTop[n] || targD >= rightBottom[n])
					return (false);
			}

			return (true);
		}

		internal bool IsInReach (IKDTreeDomain target, double distRad)
		{
			return (Distance (target) < distRad);
		}

		internal double Distance (IKDTreeDomain target)
		{
			int closestPointN;
			int distance = 0;

			for (int n = 0 ; n < dim ; ++n) {
				int tI = target.GetDimensionElement (n);
				int hrMin = leftTop[n];
				int hrMax = rightBottom[n];

				closestPointN = 0;
				if (tI <= hrMin) {
					closestPointN = hrMin;
				} else if (tI > hrMin && tI < hrMax) {
					closestPointN = tI;
				} else if (tI >= hrMax) {
					closestPointN = hrMax;
				}

				int dimDist = tI - closestPointN;
				distance += dimDist * dimDist;
			}

			return (Math.Sqrt ((double) distance));
		}
	}

	public IKDTreeDomain NearestNeighbour (IKDTreeDomain target, out double resDist)
	{
		HyperRectangle hr =
			HyperRectangle.CreateUniverseRectangle (target.DimensionCount);

		IKDTreeDomain nearest = NearestNeighbourI (target, hr,
			Double.PositiveInfinity, out resDist);
		resDist = Math.Sqrt (resDist);

		return (nearest);
	}


	private IKDTreeDomain NearestNeighbourI (IKDTreeDomain target, HyperRectangle hr,
		double maxDistSq, out double resDistSq)
	{
		resDistSq = Double.PositiveInfinity;

		IKDTreeDomain pivot = dr;

		HyperRectangle leftHr = hr;
		HyperRectangle rightHr = leftHr.SplitAt (splitDim,
			pivot.GetDimensionElement (splitDim));

		HyperRectangle nearerHr, furtherHr;
		KDTree nearerKd, furtherKd;

		if (target.GetDimensionElement (splitDim) <=
			pivot.GetDimensionElement (splitDim))
		{
			nearerKd = left;
			nearerHr = leftHr;
			furtherKd = right;
			furtherHr = rightHr;
		} else {
			nearerKd = right;
			nearerHr = rightHr;
			furtherKd = left;
			furtherHr = leftHr;
		}

		IKDTreeDomain nearest = null;
		double distSq;
		if (nearerKd == null) {
			distSq = Double.PositiveInfinity;
		} else {
			nearest = nearerKd.NearestNeighbourI (target, nearerHr,
				maxDistSq, out distSq);
		}

		maxDistSq = Math.Min (maxDistSq, distSq);

		if (furtherHr.IsInReach (target, Math.Sqrt (maxDistSq))) {
			double ptDistSq = KDTree.DistanceSq (pivot, target);
			if (ptDistSq < distSq) {
				nearest = pivot;
				distSq = ptDistSq;
				maxDistSq = distSq;
			}

			double tempDistSq;
			IKDTreeDomain tempNearest = null;
			if (furtherKd == null) {
				tempDistSq = Double.PositiveInfinity;
			} else {
				tempNearest = furtherKd.NearestNeighbourI (target,
					furtherHr, maxDistSq, out tempDistSq);
			}

			if (tempDistSq < distSq) {
				nearest = tempNearest;
				distSq = tempDistSq;
			}
		}

		resDistSq = distSq;
		return (nearest);
	}

	public ArrayList NearestNeighbourList (IKDTreeDomain target,
		out double resDist, int q)
	{
		HyperRectangle hr =
			HyperRectangle.CreateUniverseRectangle (target.DimensionCount);

		SortedLimitedList best = new SortedLimitedList (q);

		IKDTreeDomain nearest = NearestNeighbourListI (best, q, target, hr,
			Double.PositiveInfinity, out resDist);
		resDist = Math.Sqrt (resDist);

		foreach (BestEntry be in best)
			be.Distance = Math.Sqrt (be.Distance);

		return (best);
	}


	private IKDTreeDomain NearestNeighbourListI (SortedLimitedList best,
		int q, IKDTreeDomain target, HyperRectangle hr, double maxDistSq,
		out double resDistSq)
	{
		resDistSq = Double.PositiveInfinity;

		IKDTreeDomain pivot = dr;

		best.Add (new BestEntry (dr, KDTree.DistanceSq (target, dr)));

		HyperRectangle leftHr = hr;
		HyperRectangle rightHr = leftHr.SplitAt (splitDim,
			pivot.GetDimensionElement (splitDim));

		HyperRectangle nearerHr, furtherHr;
		KDTree nearerKd, furtherKd;

		if (target.GetDimensionElement (splitDim) <=
			pivot.GetDimensionElement (splitDim))
		{
			nearerKd = left;
			nearerHr = leftHr;
			furtherKd = right;
			furtherHr = rightHr;
		} else {
			nearerKd = right;
			nearerHr = rightHr;
			furtherKd = left;
			furtherHr = leftHr;
		}

		IKDTreeDomain nearest = null;
		double distSq;

		if (nearerKd == null) {
			distSq = Double.PositiveInfinity;
		} else {
			nearest = nearerKd.NearestNeighbourListI (best, q, target, nearerHr,
				maxDistSq, out distSq);
		}

		if (best.Count >= q)
			maxDistSq = ((BestEntry) best[q - 1]).Distance;
		else
			maxDistSq = Double.PositiveInfinity;

		if (furtherHr.IsInReach (target, Math.Sqrt (maxDistSq))) {
			double ptDistSq = KDTree.DistanceSq (pivot, target);
			if (ptDistSq < distSq) {
				nearest = pivot;
				distSq = ptDistSq;

				maxDistSq = distSq;
			}

			double tempDistSq;
			IKDTreeDomain tempNearest = null;
			if (furtherKd == null) {
				tempDistSq = Double.PositiveInfinity;
			} else {
				tempNearest = furtherKd.NearestNeighbourListI (best, q, target,
					furtherHr, maxDistSq, out tempDistSq);
			}

			if (tempDistSq < distSq) {
				nearest = tempNearest;
				distSq = tempDistSq;
			}
		}

		resDistSq = distSq;
		return (nearest);
	}

	public ArrayList NearestNeighbourListBBF (IKDTreeDomain target,
		int q, int searchSteps)
	{
		HyperRectangle hr =
			HyperRectangle.CreateUniverseRectangle (target.DimensionCount);

		SortedLimitedList best = new SortedLimitedList (q);
		SortedLimitedList searchHr = new SortedLimitedList (searchSteps);

		int dummyDist;
		IKDTreeDomain nearest = NearestNeighbourListBBFI (best, q, target, hr,
			Int32.MaxValue, out dummyDist, searchHr, ref searchSteps);

		foreach (BestEntry be in best)
			be.Distance = Math.Sqrt (be.DistanceSq);

		return (best);
	}


	private IKDTreeDomain NearestNeighbourListBBFI (SortedLimitedList best,
		int q, IKDTreeDomain target, HyperRectangle hr, int maxDistSq,
		out int resDistSq, SortedLimitedList searchHr, ref int searchSteps)
	{
		resDistSq = Int32.MaxValue;

		IKDTreeDomain pivot = dr;

		best.Add (new BestEntry (dr, KDTree.DistanceSq (target, dr), true));

		HyperRectangle leftHr = hr;
		HyperRectangle rightHr = leftHr.SplitAt (splitDim,
			pivot.GetDimensionElement (splitDim));

		HyperRectangle nearerHr, furtherHr;
		KDTree nearerKd, furtherKd;

		if (target.GetDimensionElement (splitDim) <=
			pivot.GetDimensionElement (splitDim))
		{
			nearerKd = left;
			nearerHr = leftHr;
			furtherKd = right;
			furtherHr = rightHr;
		} else {
			nearerKd = right;
			nearerHr = rightHr;
			furtherKd = left;
			furtherHr = leftHr;
		}

		IKDTreeDomain nearest = null;
		int distSq;

		searchHr.Add (new HREntry (furtherHr, furtherKd, pivot,
			furtherHr.Distance (target)));

		if (nearerKd == null) {
			distSq = Int32.MaxValue;
		} else {
			nearest = nearerKd.NearestNeighbourListBBFI (best, q, target, nearerHr,
				maxDistSq, out distSq, searchHr, ref searchSteps);
		}

		if (best.Count >= q) {
			maxDistSq = ((BestEntry) best[q - 1]).DistanceSq;
		} else
			maxDistSq = Int32.MaxValue;

		if (searchHr.Count > 0) {
			HREntry hre = (HREntry) searchHr[0];
			searchHr.RemoveAt (0);

			furtherHr = hre.HR;
			furtherKd = hre.Tree;
			pivot = hre.Pivot;
		}

		searchSteps -= 1;
		if (searchSteps > 0 &&
			furtherHr.IsInReach (target, Math.Sqrt (maxDistSq)))
		{
			int ptDistSq = KDTree.DistanceSq (pivot, target);
			if (ptDistSq < distSq) {
				nearest = pivot;
				distSq = ptDistSq;

				maxDistSq = distSq;
			}

			int tempDistSq;
			IKDTreeDomain tempNearest = null;
			if (furtherKd == null) {
				tempDistSq = Int32.MaxValue;
			} else {
				tempNearest = furtherKd.NearestNeighbourListBBFI (best, q,
					target, furtherHr, maxDistSq, out tempDistSq, searchHr,
					ref searchSteps);
			}

			if (tempDistSq < distSq) {
				nearest = tempNearest;
				distSq = tempDistSq;
			}
		}

		resDistSq = distSq;
		return (nearest);
	}


	public static int DistanceSq (IKDTreeDomain t1, IKDTreeDomain t2)
	{
		int distance = 0;

		for (int n = 0 ; n < t1.DimensionCount ; ++n) {
			int dimDist = t1.GetDimensionElement (n) -
				t2.GetDimensionElement (n);
			distance += dimDist * dimDist;
		}

		return (distance);
	}

	static private IKDTreeDomain GoodCandidate (ArrayList exset, out int splitDim)
	{
		IKDTreeDomain first = exset[0] as IKDTreeDomain;
		if (first == null) {
			Console.WriteLine ("type: {0}", exset[0]);
			throw (new Exception ("Not of type IKDTreeDomain (TODO: custom exception)"));
		}

		int dim = first.DimensionCount;

		double[] minHr = new double[dim];
		double[] maxHr = new double[dim];
		for (int k = 0 ; k < dim ; ++k) {
			minHr[k] = Double.PositiveInfinity;
			maxHr[k] = Double.NegativeInfinity;
		}

		foreach (IKDTreeDomain dom in exset) {
			for (int k = 0 ; k < dim ; ++k) {
				double dimE = dom.GetDimensionElement (k);

				if (dimE < minHr[k])
					minHr[k] = dimE;
				if (dimE > maxHr[k])
					maxHr[k] = dimE;
			}
		}

		double[] diffHr = new double[dim];
		int maxDiffDim = 0;
		double maxDiff = 0.0;
		for (int k = 0 ; k < dim ; ++k) {
			diffHr[k] = maxHr[k] - minHr[k];
			if (diffHr[k] > maxDiff) {
				maxDiff = diffHr[k];
				maxDiffDim = k;
			}
		}

		double middle = (maxDiff / 2.0) + minHr[maxDiffDim];
		IKDTreeDomain exemplar = null;
		double exemMinDiff = Double.PositiveInfinity;

		foreach (IKDTreeDomain dom in exset) {
			double curDiff = Math.Abs (dom.GetDimensionElement (maxDiffDim) - middle);
			if (curDiff < exemMinDiff) {
				exemMinDiff = curDiff;
				exemplar = dom;
			}
		}

		splitDim = maxDiffDim;

		return (exemplar);
	}

	static public KDTree CreateKDTree (ArrayList exset)
	{
		if (exset.Count == 0)
			return (null);

		KDTree cur = new KDTree ();
		cur.dr = GoodCandidate (exset, out cur.splitDim);

		ArrayList leftElems = new ArrayList ();
		ArrayList rightElems = new ArrayList ();

		double bound = cur.dr.GetDimensionElement (cur.splitDim);
		foreach (IKDTreeDomain dom in exset) {

			if (dom == cur.dr)
				continue;

			if (dom.GetDimensionElement (cur.splitDim) <= bound) {
				leftElems.Add (dom);
			} else {
				rightElems.Add (dom);
			}
		}

		cur.left = KDTree.CreateKDTree (leftElems);
		cur.right = KDTree.CreateKDTree (rightElems);

		return (cur);
	}

	public interface IKDTreeDomain
	{
		int DimensionCount {
			get;
		}
		int GetDimensionElement (int dim);
	}
}


