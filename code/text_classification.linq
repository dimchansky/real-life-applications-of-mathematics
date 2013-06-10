<Query Kind="Program">
  <Reference Relative="NaiveBayes\bin\Release\NaiveBayes.dll">D:\Programming\Math\Real life applications of mathematics\code\NaiveBayes\bin\Release\NaiveBayes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Numerics.dll</Reference>
  <Namespace>NaiveBayes</Namespace>
</Query>

void Main()
{
	var trainData = new TrainingData<string, string>
	{
		{ "spam", "offer is secret" },
		{ "spam", "click secret link" },
		{ "spam", "secret sport link" },
		{ "ham", "play sport today" },
		{ "ham", "went play sport" },
		{ "ham", "secret sport event" },
		{ "ham", "sport is today" },
		{ "ham", "sport costs money" },
	};
	
	var trainData2 = TrainingData.Create(trainData.Select(pair => Tuple.Create(pair.Item1, pair.Item2.Split())));
	
	var naiveBayes = NaiveBayes.NaiveBayes.Create(trainData2, 1);
	WriteInfo(naiveBayes);
	WriteCategoryProbability(naiveBayes, "spam");
	WriteCategoryProbability(naiveBayes, "ham");
	WriteAttributeProbabilityGivenCategory(naiveBayes, "today", "spam");
	WriteAttributeProbabilityGivenCategory(naiveBayes, "is", "spam");
	WriteAttributeProbabilityGivenCategory(naiveBayes, "secret", "spam");
	WriteAttributeProbabilityGivenCategory(naiveBayes, "today", "ham");	
	WriteAttributeProbabilityGivenCategory(naiveBayes, "is", "ham");		
	WriteAttributeProbabilityGivenCategory(naiveBayes, "secret", "ham");	
	WriteCategoryProbabilityGivenAttributes(naiveBayes, "spam", new[] { "today", "is", "secret" });	
}

private static void WriteInfo<TCategory, TAttribute>(NaiveBayes<TCategory, TAttribute> naiveBayes)
{
	Console.WriteLine("Laplace smoothing k=" + naiveBayes.LaplaceSmoothingK);
}

private static void WriteCategoryProbabilityGivenAttributes<TCategory, TAttribute>(NaiveBayes<TCategory, TAttribute> naiveBayes, TCategory category, TAttribute[] attributes)
{
	var probability = naiveBayes.GetCategoryProbabilityGivenAttributes(category, attributes);
	Console.WriteLine("P({0}|{1})={2} (={3})", category, string.Join(",",attributes), probability, (double)probability);
}

private static void WriteCategoryProbability<TCategory, TAttribute>(NaiveBayes<TCategory, TAttribute> naiveBayes, TCategory category)
{
	var probability = naiveBayes.GetCategoryProbability(category);

	Console.WriteLine("P({0}) = {1} (={2})", category, probability, (double)probability);
}

private static void WriteAttributeProbabilityGivenCategory<TCategory, TAttribute>(NaiveBayes<TCategory, TAttribute> naiveBayes, TAttribute attribute, TCategory category)
{
	var probability = naiveBayes.GetAttributeProbabilityGivenCategory(attribute, category);

	Console.WriteLine("P({0}|{1}) = {2} (={3})", attribute, category, probability, (double)probability);
}
