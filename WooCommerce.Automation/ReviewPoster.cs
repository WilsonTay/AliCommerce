using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CookComputing.XmlRpc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;
using WordPressSharp;
using WordPressSharp.Models;

namespace WooCommerce.Automation
{
    public class ReviewBotRunner
    {
        private readonly WCObject wc;
        private readonly WordPressClient wpClient;
        private readonly ILogger logger;
        private MySqlConnection connection;

        public ReviewBotRunner(
            string restAPIKey,
            string restAPISecret,
            WordPressClient wpClient,
            string wordpressConnectionString,
            ILogger logger)
        {
            RestAPI rest = new RestAPI("http://dealswhat.com/wc-api/v3/",
            restAPIKey,
            restAPISecret);
            this.wc = new WCObject(rest);
            this.wpClient = wpClient;
            this.logger = logger;
            this.connection = new MySqlConnection(wordpressConnectionString);
            this.connection.Open();
        }

        private void PostReview(string postId, string author, string content, double rating)
        {

            var checkCommentDupFormat = "select * from wp_comments where comment_author='{0}'";
            var checkCommentDupQuery = string.Format(checkCommentDupFormat, author);
            MySqlCommand dupCmd = new MySqlCommand(checkCommentDupQuery, connection);
            using (var reader = dupCmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    this.logger.Info("Duplicate review.");
                    return;
                }
            }

            var commentId = this.wpClient
                                .NewComment(new Comment
                                {
                                    Content = content,
                                    PostId = postId
                                });
        
            var queryFormat =
                "update wp_comments set comment_author='{0}' , user_id='999' , comment_author_email='' where comment_id='{1}'";

            var insertQueryFormat = "insert into wp_commentmeta(comment_id,meta_key,meta_value) values('{0}','rating','{1}')";

            var query = string.Format(queryFormat, author, commentId);
            var insertQuery = string.Format(insertQueryFormat, commentId, rating);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();

            MySqlCommand cmd1 = new MySqlCommand(insertQuery, connection);
            cmd1.ExecuteNonQuery();
        }

        public void Run()
        {
            // call to woocommerce rest api
            // get list of published products
            var products = wc.GetProducts(new Dictionary<string, string>
                {
                        {"filter[limit]","10000" },
                    {"filter[post_status]","publish" }


                }).Result;

            this.logger.Info("Retrieved " + products.Count + " published products.");

            foreach (var product in products)
            {
                // get list of reviews
                var woocommerceReviews = this.wc.GetProductReviews(product.id, new Dictionary<string, string>()).Result;

                this.logger.Info("Product id " + product.id + " with title " + product.title + " has " + woocommerceReviews.Count + " reviews.");

                var today = DateTime.UtcNow;
                var hasReviewsToday =
                    woocommerceReviews.Any(
                        a =>
                            a.created_at.Day == today.Day &&
                            a.created_at.Month == today.Month &&
                            a.created_at.Year == today.Year);

                if (!hasReviewsToday)
                {
                    this.logger.Info(
                        "No review has been posted for this product today. We are progressing to post new reviews..");
                    // with ProductId

                    // call to aliexpress reviews endpoint by passing in product id
                    // get only reviews today
                    for (int page = 0; page < 10; page++)
                    {
                        var extractor = new AliExpressReviewExtractor();

                        this.logger.Info("Trying to grab reviews on aliexpress page " + page);
                        var reviews = extractor.GetReviews(product.sku, page).ToList();

                        var reviewsYesterday = reviews.Where(a =>
                        {
                            var dt = DateTime.Parse(a.gmtValid);
                            return dt.Day == today.Day - 1 &&
                                   dt.Month == today.Month &&
                                   dt.Year == today.Year;
                        })
                        .Distinct()
                        .ToList();



                        // If no yesterday's review in this page then stop progressing.
                        if (!reviewsYesterday.Any())
                        {
                            this.logger.Info("Shit. No review yesterday. We are skipping this.");
                            break;
                        }
                        else
                        {
                            this.logger.Info("Wow. We found " + reviews.Count + " yesterday's reviews.");
                        }

                        foreach (var review in reviewsYesterday)
                        {
                            // if review is from russia
                            // then call to google translate api
                            // var translater = new GoogleTranslater();
                            // var translatedContent = translater.Translate(review.buyerFeedback);

                            // post to wordpress comment xmlrpc get the id
                            // after posting go db and change the user id to 999 and poster name

                            // add a comment meta on review
                            if (!ContainsUnicodeCharacter(review.buyerFeedback))
                            {
                                this.logger.Info("Posting review " + review.buyerName + " with feedback " +
                                                 review.buyerFeedback + " rating " + review.buyerEval);
                                try
                                {
                                    PostReview(product.id.ToString(), review.buyerName, review.buyerFeedback,
                                        double.Parse(review.buyerEval) / 20);

                                    this.logger.Info("Successully posted review.");
                                }
                                catch (XmlRpcFaultException ex)
                                {
                                    this.logger.Info("Duplicate review.");
                                }

                            }
                            else
                            {
                                this.logger.Info("Review contains unicode character. We are skipping this.");
                            }
                        }
                    }
                }
                else
                {
                    this.logger.Info("Reviews have been posted for this product. We are skipping this.");
                }
            }
        }

        private bool ContainsUnicodeCharacter(string input)
        {

            return input.Any(c => c > 255);
        }
    }

    public class ReviewPoster
    {


    }

    public class AliExpressReviewExtractor
    {
        private string endpointFormat =
            "http://m.aliexpress.com/evaluation/ajax/getProductEvaluationDetail.htm?productId={0}&page={1}";

        public List<AliExpressReview> GetReviews(string productId, int page)
        {
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var endpoint = string.Format(endpointFormat, productId, page);
            var response = webClient.DownloadString(endpoint);

            var reviews = JsonConvert.DeserializeObject<List<AliExpressReview>>(response);

            return reviews;
        }
    }

    public class AliExpressReview
    {
        public string gmtValid { get; set; }
        public string buyerCountry { get; set; }
        public string buyerEval { get; set; }
        public string buyerFeedback { get; set; }

        public string buyerName { get; set; }
    }

    public class GoogleTranslater
    {
        private string endpointFormat =
            "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={0}";

        public string Translate(string original)
        {
            var endpoint = string.Format(endpointFormat, HttpUtility.UrlEncode(original));
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            var response = wc.DownloadString(endpoint);

            var jArray = JArray.Parse(response);
            var firstArray = jArray[0];

            var allSentences = firstArray.Children();
            var str = "";

            foreach (var sentence in allSentences)
            {
                str += sentence.First.Value<string>();

            }

            Console.WriteLine(allSentences);

            return str;
        }
    }
}
