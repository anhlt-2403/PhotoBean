namespace Models;

public class ContentResponse
{
    public string Id { get; set; }
    public string Project { get; set; }
    public string Iteration { get; set; }
    public string Created { get; set; }
    public ICollection<ProbabilityDetail> Predictions { get; set; }
}
