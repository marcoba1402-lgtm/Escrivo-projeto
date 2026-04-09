using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class JogoController : MonoBehaviour
{
    public TextMeshProUGUI textLetra;
    public TextMeshProUGUI textTema;
    public TextMeshProUGUI textDica;
    public TextMeshProUGUI textTentativas; // agora mostra "Vidas"
    public TextMeshProUGUI textRodada;
    public TMP_InputField inputField;
    public TextMeshProUGUI textPontuacao;

    public AudioSource audioSource;
    public AudioClip somAcerto;
    public AudioClip somErro;

    int vidas; // antes era tentativas
    int indiceDica;
    int rodada = 1;
    int maxRodadas = 3;
    int pontuacao = 0;

    string palavraCorreta;
    string[,] palavrasAtuais;
    string[] dicasAtuais;

    bool jogoAtivo = false;

    void Start()
    {
        string dificuldade = PlayerPrefs.GetString("dificuldade", "facil");

        if (dificuldade == "medio")
            palavrasAtuais = palavrasMedio;
        else if (dificuldade == "dificil")
            palavrasAtuais = palavrasDificil;
        else
            palavrasAtuais = palavrasFacil;

        inputField.lineType = TMP_InputField.LineType.SingleLine;
        inputField.onSubmit.AddListener(OnSubmitMobile);

        inputField.gameObject.SetActive(false);
        textPontuacao.text = "Pontos: " + pontuacao;

        AtualizarRodada();
        StartCoroutine(RodarRoleta());
    }

    void AtualizarRodada()
    {
        textRodada.text = "Rodada: " + rodada + "/" + maxRodadas;
    }

    IEnumerator RodarRoleta()
    {
        jogoAtivo = false;

        vidas = 3; // inicializa as vidas
        indiceDica = 0;

        textTentativas.text = "Vidas: " + vidas;
        textDica.text = "";

        string[] letras = {
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
        };

        float tempo = 0.05f;

        for (int i = 0; i < 40; i++)
        {
            textLetra.text = letras[Random.Range(0, 26)];

            if (i > 25) tempo += 0.02f;

            yield return new WaitForSeconds(tempo);
        }

        CarregarPalavra();

        jogoAtivo = true;

        inputField.gameObject.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void CarregarPalavra()
    {
        int i = Random.Range(0, palavrasAtuais.GetLength(0));

        palavraCorreta = palavrasAtuais[i, 2];

        textLetra.text = palavrasAtuais[i, 0];
        textTema.text = "Tema: " + palavrasAtuais[i, 1];

        dicasAtuais = new string[3];
        dicasAtuais[0] = palavrasAtuais[i, 3];
        dicasAtuais[1] = palavrasAtuais[i, 4];
        dicasAtuais[2] = palavrasAtuais[i, 5];

        AtualizarDicas();
    }

    void AtualizarDicas()
    {
        string texto = "Dicas:\n";

        for (int i = 0; i <= indiceDica; i++)
        {
            texto += (i + 1) + " - " + dicasAtuais[i] + "\n";
        }

        textDica.text = texto;
    }

    void OnSubmitMobile(string texto)
    {
        VerificarResposta();
    }

    public void BotaoEnviar()
    {
        VerificarResposta();
    }

    public void VerificarResposta()
    {
        if (!jogoAtivo) return;

        string resposta = inputField.text.ToUpper().Trim();
        if (resposta == "") return;

        if (resposta == palavraCorreta)
        {
            pontuacao += 100;
            textPontuacao.text = "Pontos: " + pontuacao;
            if (audioSource != null && somAcerto != null)
                audioSource.PlayOneShot(somAcerto);

            jogoAtivo = false;
            inputField.gameObject.SetActive(false);

            textDica.text = "ACERTOU!";

            rodada++;

            if (rodada > maxRodadas)
                StartCoroutine(IrParaVitoria());
            else
            {
                AtualizarRodada();
                StartCoroutine(ProximaRodada());
            }
        }
        else
        {
            if (audioSource != null && somErro != null)
                audioSource.PlayOneShot(somErro);

            vidas--; // decrementa vida
            pontuacao -= 20;
            if (pontuacao < 0) pontuacao = 0;

            textPontuacao.text = "Pontos: " + pontuacao;
            textTentativas.text = "Vidas: " + vidas;

            if (vidas <= 0)
            {
                jogoAtivo = false;
                inputField.gameObject.SetActive(false);

                textDica.text = "ERROU! Era: " + palavraCorreta;

                StartCoroutine(IrParaDerrota());
                return;
            }

            indiceDica++;
            if (indiceDica > 2) indiceDica = 2;

            AtualizarDicas();
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    IEnumerator ProximaRodada()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(RodarRoleta());
    }

    IEnumerator IrParaVitoria()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Vitoria");
    }

    IEnumerator IrParaDerrota()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Derrota");
    }

    void Update()
    {
        if (!jogoAtivo) return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                VerificarResposta();
            }
        }
    }

    // ===== BANCO DE PALAVRAS SUPER FÁCIL =====

    string[,] palavrasFacil = new string[,]
{
    // A
    {"A","Animal","ABELHA","faz mel","tem listras","voa de flor em flor"},
    {"A","Fruta","ABACAXI","tem coroa","tem casca grossa","e muito azedo"},
    {"A","Objeto","ANEL","usa no dedo","pode ser de ouro","enfeita a mao"},
    // B
    {"B","Brinquedo","BONECA","parece um bebe","as meninas gostam","fica na casinha"},
    {"B","Objeto","BOLA","e redonda","usada no futebol","pula e rola"},
    {"B","Animal","BORBOLETA","tem asas coloridas","ja foi lagarta","voa no jardim"},
    // C
    {"C","Animal","CAVALO","tem quatro patas","faz galope","vive na fazenda"},
    {"C","Objeto","CADEIRA","serve para sentar","tem quatro pes","fica na mesa"},
    {"C","Animal","CACHORRO","amigo do homem","late alto","gosta de osso"},
    // D
    {"D","Parte do Corpo","DENTE","fica na boca","usado para morder","e muito branco"},
    {"D","Jogo","DOMINÓ","pecas com pintinhas","jogo de mesa","tem a peca de sena"},
    {"D","Parte do Corpo","DEDO","tem unha na ponta","temos dez nas maos","usa alianca"},
    // E
    {"E","Objeto","ESCOVA","limpa os dentes","penteia o cabelo","tem cerdas"},
    {"E","Animal","ELEFANTE","e muito grande","tem uma tromba","tem orelhas enormes"},
    {"E","Objeto","ESPELHO","reflete a imagem","mostra o rosto","pode quebrar"},
    // F
    {"F","Comida","FEIJAO","pretinho ou carioquinha","come com arroz","tem caldo quente"},
    {"F","Natureza","FOGO","e muito quente","faz fumaça","queima a lenha"},
    {"F","Objeto","FACA","usa para cortar","fica na cozinha","e muito afiada"},
    // G
    {"G","Animal","GATO","faz miau","tem bigodes","gosta de leite"},
    {"G","Animal","GALINHA","poe ovo","tem penas","faz cocoricó"},
    {"G","Animal","GIRAFA","tem pescoco comprido","e muito alta","tem manchas na pele"},
    // H
    {"H","Meio de Transporte","HELICOPTERO","voa no ar","tem helice","nao e aviao"},
    {"H","Lugar","HOSPITAL","onde estao os medicos","cuida dos doentes","tem ambulancia"},
    {"H","Comida","HAMBURGUER","vai no pao","tem carne e queijo","comida de lanche"},
    // I
    {"I","Lugar","IGREJA","tem sino","lugar de orar","tem cruz no topo"},
    {"I","Brinquedo","IOIÔ","sobe e desce","tem um barbante","enrola na mao"},
    {"I","Lugar","ILHA","terra no meio do mar","tem coqueiros","chega de barco"},
    // J
    {"J","Animal","JACARE","vive na agua","tem bocao","tem escamas"},
    {"J","Casa","JANELA","abre para entrar sol","tem vidro","olha para a rua"},
    {"J","Natureza","JARDIM","tem muitas flores","lugar de borboleta","tem que regar"},
    // L
    {"L","Fruta","LARANJA","cor de fruta","faz suco","tem vitamina C"},
    {"L","Objeto","LAPIS","usa para escrever","tem grafite","pode apontar"},
    {"L","Animal","LEAO","rei da selva","tem uma juba","da um rugido alto"},
    // M
    {"M","Animal","MACACO","gosta de banana","pula em arvore","faz careta"},
    {"M","Fruta","MELANCIA","grande e pesada","verde por fora","vermelha por dentro"},
    {"M","Parte do Corpo","MAO","tem cinco dedos","usa para acenar","pega as coisas"},
    // N
    {"N","Natureza","NUVEM","fica no ceu","parece algodao","solta chuva"},
    {"N","Meio de Transporte","NAVIO","anda na agua","e muito grande","leva passageiros"},
    {"N","Parte do Corpo","NARIZ","serve para cheirar","fica no rosto","espirra quando coça"},
    // O
    {"O","Comida","OVO","vem da galinha","tem gema","pode ser frito"},
    {"O","Parte do Corpo","OLHO","serve para ver","fica na cara","a gente pisca"},
    {"O","Objeto","OCULOS","ajuda a enxergar","vai no rosto","tem duas lentes"},
    // P
    {"P","Comida","PIPOCA","estoura na panela","come no cinema","e de milho"},
    {"P","Animal","PEIXE","vive na agua","tem escamas","sabe nadar"},
    {"P","Brinquedo","PIÃO","roda no chao","tem uma ponta","enrola o barbante"},
    // Q
    {"Q","Comida","QUEIJO","amarelo ou branco","rato gosta","vai no pao"},
    {"Q","Lugar","QUINTAL","fica atras da casa","lugar de brincar","tem terra e sol"},
    {"Q","Objeto","QUADRO","enfeita a parede","tem moldura","pode ser uma pintura"},
    // R
    {"R","Animal","RATO","tem medo de gato","gosta de queijo","tem rabo comprido"},
    {"R","Objeto","RELOGIO","mostra as horas","faz tic tac","fica no pulso"},
    {"R","Lugar","RIO","tem agua corrente","tem peixes","corre para o mar"},
    // S
    {"S","Animal","SAPO","verde e pula","vive na lagoa","come mosca"},
    {"S","Calçado","SAPATO","poe no pe","usa para andar","tem cadarço"},
    {"S","Natureza","SOL","brilha no ceu","aquece o dia","aparece de manha"},
    // T
    {"T","Objeto","TELEFONE","usado para ligar","tem numero","faz trim trim"},
    {"T","Animal","TARTARUGA","anda bem devagar","tem um casco","vive muitos anos"},
    {"T","Objeto","TESOURA","usa para cortar","tem duas laminas","nao e brinquedo"},
    // U
    {"U","Fruta","UVA","pequena e redonda","pode ser verde ou roxa","cresce em cachos"},
    {"U","Animal","URSO","e muito peludo","gosta de mel","dorme no inverno"},
    {"U","Parte do Corpo","UNHA","fica na ponta do dedo","a gente corta","pode pintar"},
    // V
    {"V","Animal","VACA","da leite","faz muu","tem chifres"},
    {"V","Objeto","VASSOURA","usa para varrer","tem cabo longo","limpa a casa"},
    {"V","Vestuário","VESTIDO","roupa de menina","e bem bonito","usa em festa"},
    // Z
    {"Z","Animal","ZEBRA","tem listras","parece cavalo","preto e branco"},
    {"Z","Lugar","ZOO","tem muitos animais","lugar de passeio","tem jaulas"},
    {"Z","Barulho","ZUMBIDO","barulho da abelha","faz no ouvido","parece um motor"}
};
    string[,] palavrasMedio = new string[,]
    {
    // A
    {"A","Objeto","AMPULHETA","Tem areia","Feito de vidro","Mede o tempo"},
    {"A","Natureza","ARVORE","tem raiz e galhos","produz oxigenio","da sombra"},
    {"A","Corpo","ABDOMEN","Entre o torax e a pelve","Tem o nome em um treino","umbigo"},
    // B
    {"B","Objeto","BINOCULO","Usa no olho","Tem lente","Serve pra ver de longe"},
    {"B","Objeto","BALDE","recipiente cilindrico","carrega liquido","tem alca"},
    {"B","Animal","BALEIA","maior animal do mundo","mamifero marinho","respira pelo espiráculo"},
    // C
    {"C","Objeto","CADEIRA","movel para sentar","tem encosto","pode ter quatro pernas"},
    {"C","Natureza","CAVERNA","buraco na rocha","escura por dentro","pode ter morcegos"},
    {"C","Corpo","COTOVELO","articulacao do braco","une umero e radio","dobra o braco"},
    // D
    {"D","Natureza","DESERTO","regiao muito seca","pouca chuva","tem dunas de areia"},
    {"D","Objeto","DADO","cubo com numeros","usado em jogos","tem seis faces"},
    {"D","Animal","DROMEDARIO","camelo com uma corcova","vive no deserto","animal de carga"},
    // E
    {"E","Objeto","ESPELHO","superficie reflexiva","feito de vidro","mostra o reflexo"},
    {"E","Natureza","ECLIPSE","fenomeno astronomico","lua ou sol some","sombra no espaco"},
    {"E","Lugar","ESTACAO","local de embarque","trem para aqui","tem plataforma"},
    // F
    {"F","Objeto","FACA","lamina afiada","usada para cortar","fica na cozinha"},
    {"F","Natureza","FURACAO","tempestade circular","vento muito forte","olho no centro"},
    {"F","Animal","FALCAO","ave de rapina","voa muito alto","caca com as garras"},
    // G
    {"G","Animal","GOLFINHO","mamifero marinho","inteligente","nada em grupo"},
    {"G","Natureza","GENGIBRE","Planta medicinal","Planta herbácea","caule subterrâneo"},
    {"G","Objeto","GUITARRA","instrumento de cordas","Tem uma variante: (double neck) que possui um total de 18 cordas","tem corpo eletrico"},
    // H
    {"H","Lugar","HOSPITAL","local de tratamento","tem medicos","atende doentes"},
    {"H","Animal","HIENA","animal africano","ri quando ameacada","come carniceiros"},
    {"H","Objeto","HELICE","pe giratoria","move aviao ou barco","gira em alta velocidade"},
    // I
    {"I","Lugar","ILHA","terra cercada de agua","pode ter praia","fica no mar"},
    {"I","Natureza","ICEBERG","bloco de gelo flutuante","maior parte submersa","flutua no oceano"},
    {"I","Animal","IGUANA","reptil verde","tem crista no dorso","come folhas"},
    // J
    {"J","Objeto","JANELA","abertura na parede","tem vidro","deixa entrar luz"},
    {"J","Lugar","JARDIM","espaco com plantas","tem flores","precisa de cuidado"},
    {"J","Animal","JAGUAR","felino manchado","vive na selva","maior felino das americas"},
    // L
    {"L","Objeto","LAPIS","instrumento de escrita","tem grafite","pode ser apagado"},
    {"L","Natureza","LAVA","rocha derretida","sai do vulcao","muito quente"},
    {"L","Animal","LONTRA","mamifero semiacuatico","vive em rios","usa pedra para abrir comida"},
    // M
    {"M","Objeto","MARTELO","ferramenta pesada","bate prego","tem cabo de madeira"},
    {"M","Natureza","MARE","movimento do oceano","causada pela lua","sobe e desce"},
    {"M","Animal","MORCEGO","unico mamifero voador","ativo a noite","usa ecolocalização"},
    // N
    {"N","Natureza","NEVE","precipitacao gelada","floco de agua congelada","cobre o chao de branco"},
    {"N","Animal","NARVAL","baleia com chifre","vive no artico","chifre e um dente"},
    {"N","Objeto","NAVIO","grande embarcacao","viaja pelo mar","tem convés"},
    // O
    {"O","Corpo","OUVIDO","orgao da audicao","capta vibracao sonora","fica na cabeca"},
    {"O","Animal","ORNITORRINCO","mamifero que bota ovo","tem bico de pato","vive na australia"},
    {"O","Natureza","OCEANO","grande massa de agua","cobre 70% da terra","tem correntes"},
    // P
    {"P","Objeto","PORTA","divisoria de ambientes","tem dobricas","abre e fecha"},
    {"P","Animal","POLVO","molusco marinho","tem oito tentaculos","solta tinta"},
    {"P","Natureza","PLANALTO","area elevada e plana","acima do nivel do mar","diferente de montanha"},
    // Q
    {"Q","Objeto","QUADRO","objeto decorativo","fica na parede","pode ter pintura"},
    {"Q","Lugar","QUILOMBO","comunidade historica","formada por escravos","resistencia cultural"},
    {"Q","Natureza","QUARTZO","rocha metamorfica","formada de areia","muito resistente"},
    // R
    {"R","Objeto","RELOGIO","marcador de tempo","tem ponteiros","pode ser digital"},
    {"R","Animal","RINOCERONTE","mamifero com chifre","pele grossa","esta em extincao"},
    {"R","Natureza","RECIFE","estrutura de coral","fica no mar raso","abriga muitos peixes"},
    // S
    {"S","Natureza","SOL","estrela central","aquece a terra","brilha durante o dia"},
    {"S","Animal","SALAMANDRA","anfibio colorido","vive em lugares umidos","pode regenerar membros"},
    {"S","Objeto","SUBMARINO","veiculo subaquatico","navega abaixo da agua","usado na guerra"},
    // T
    {"T","Objeto","TESOURA","instrumento de corte","tem duas laminas","usada em papelaria"},
    {"T","Animal","TARTARUGA","reptil de casco","vive muito tempo","anda devagar"},
    {"T","Natureza","TORNADO","coluna de vento","gira em espiral","destroi construcoes"},
    // U
    {"U","Animal","URSO","mamifero de grande porte","hiberna no inverno","vive na floresta"},
    {"U","Lugar","USINA","instalacao industrial","gera energia","pode ser hidrelétrica"},
    {"U","Objeto","URNA","recipiente para votos","usada em eleicoes","tambem guarda cinzas"},
    // V
    {"V","Objeto","VASSOURA","utensilio de limpeza","tem cabo longo","varre o chao"},
    {"V","Animal","VAMPIRO","morcego sugador de sangue","ativo a noite","vive em cavernas"},
    {"V","Natureza","VULCAO","montanha que erupciona","expele lava","tem cratera no topo"},
    // X
    {"X","Objeto","XICARA","recipiente para bebidas","tem asa","usada para cafe ou cha"},
    {"X","Objeto","XADREZ","Tem 32 peças","Estrategia","Tem 32 peças"},
    {"X","Instrumento","XILOFONE","instrumento de percussao","tem barras de madeira","toca com baqueta"},
    // Z
    {"Z","Animal","ZEBRA","equino listrado","vive na savana","listras pretas e brancas"},
    {"Z","Lugar","ZOOLOGICO","parque de animais","tem especies diversas","visitado por familias"},
    {"Z","Objeto","ZEPELIM","aeronave gigante","flutua com gas","usada no seculo XX"}
    };
    string[,] palavrasDificil = new string[,]
{
    // A
    {"A","Religiao","ALTAR","fica na frente da igreja","onde o padre celebra","tem cruz"},
    {"A","Cozinha","AZEITE","oleo de azeitona","tempero de salada","vem em garrafinha"},
    {"A","Campo","ARADO","ferramenta do lavrador","puxa com boi","prepara a terra"},
    // B
    {"B","Casa","BACIA","usada para lavar roupa","de plastico ou metal","fica no quintal"},
    {"B","Futebol","BOLA","redonda","chutada no campo","tem 32 gomos"},
    {"B","Cozinha","BANHA","gordura de porco","usada para fritar","deixa a comida gostosa"},
    // C
    {"C","Religiao","CAPELA","pequena igreja","tem sino","lugar de oracao"},
    {"C","Campo","CANDELABRO","comida de gado","cresce no pasto","verde e alto"},
    {"C","Cozinha","CALIFORNIA","panela grande","cozinha muito de uma vez","tem duas alcas"},
    // D
    {"D","Casa","DESPERTADOR","acorda de manha","toca alto","tem ponteiros"},
    {"D","Cozinha","DOCE DE LEITE","marrom e cremoso","feito com leite e acucar","passa no pao"},
    {"D","Objeto","DENTADURA","Coloca na água","Se usa escova de dente","pode ser removida"},
    // E
    {"E","Religiao","ESCAPULARIO","usado no pescoco","tem imagem de santo","protecao da fe"},
    {"E","Casa","ESCOVA","limpa os dentes","usada todo dia","tem cerdas"},
    {"E","Cozinha","ESPETO","enfia na carne","usado no churrasco","de metal ou madeira"},
    // F
    {"F","Futebol","FALTA","jogador derrubou o outro","cobra com chute","o juiz apita"},
    {"F","Cozinha","FARINHA","Acompanha com feijão","feita com trigo","Grãos"},
    {"F","Campo","FOICE","corta mato","tem cabo de madeira","lamina curvada"},
    // G
    {"G","Casa","GAVETA","guarda miudezas","abre e fecha","fica no armario"},
    {"G","Cozinha","GOIABADA","doce de goiaba","come com queijo","vem em caixinha"},
    {"G","Campo","GALINHEIRO","onde ficam as galinhas","tem poleiro","cheira a esterco"},
    // H
    {"H","Casa","HAMACA","rede de descanso","balanca com o vento","amarra em duas arvores"},
    {"H","Religiao","HINARIO","livro de musicas da igreja","cantado na missa","tem letras de louvor"},
    {"H","Cozinha","HORTELÃ","erva cheirosa","usada no suco","tambem enfeita prato"},
    // I
    {"I","Religiao","IGREJA","templo de oracao","tem bancos de madeira","tem vitral"},
    {"I","Casa","ISQUEIRO","acende o fogo","tem chama","cabe no bolso"},
    {"I","Cozinha","INHAME","tuberculo roxo","cozinha na agua","come com manteiga"},
    // J
    {"J","Futebol","JUIZ","apita o jogo","da cartao amarelo","corre atras da bola"},
    {"J","Cozinha","JILÓ","verdura amarga","frito ou cozido","nem todo mundo gosta"},
    {"J","Casa","JARRA","guarda agua ou suco","de vidro ou plastico","fica na mesa"},
    // L
    {"L","Cozinha","LINGUICA","feita de porco","frita na frigideira","tem no churrasco"},
    {"L","Campo","LEITOA","porca jovem","cria no chiqueiro","da carne e toucinho"},
    {"L","Religiao","LADAINHA","reza longa","repetida em voz alta","feita em grupo"},
    // M
    {"M","Cozinha","MANDIOCA","raiz branca","faz farinha","tem no interior"},
    {"M","Casa","MAQUINA DE COSTURA","faz roupa","tem agulha e linha","faz barulho ao pedalar"},
    {"M","Campo","MILHO","planta de espiga","faz canjica e curau","comida de festa junina"},
    // N
    {"N","Casa","NOVELA","passa todo dia na tv","tem mocinha e vilao","todo mundo acompanha"},
    {"N","Futbol","NEYMAR","Maior artilheiro","Ganhou libertadores","Famoso por usar moicano"},
    {"N","Campo","NASCENTE","de onde brota a agua","agua pura e fresca","origem do rio"},
    // O
    {"O","Religiao","ORACAO","fala com deus","tem o pai nosso","faz de joelhos"},
    {"O","Cozinha","OMELETE","feita com ovo","frita na frigideira","pode ter queijo"},
    {"O","Casa","ORATÓRIO","armario com santo","tem vela acesa","fica na sala"},
    // P
    {"P","Cozinha","PAMONHA","feita de milho verde","embrulhada em palha","comida junina"},
    {"P","Campo","POMBAL","onde ficam os pombos","estrutura de madeira","fica no quintal"},
    {"P","Casa","PENEIRA","coar farinha ou liquido","de tela fina","tem armacao de madeira"},
    // Q
    {"Q","Cozinha","QUEIJO","feito de leite","come com goiabada","tem varios tipos"},
    {"Q","Campo","QUINTAL","espaco atras da casa","tem arvore frutifera","galinha cisca ali"},
    {"Q","Casa","QUARTO","onde se dorme","tem cama e guarda roupa","lugar de descanso"},
    // R
    {"R","Futebol","RESERVA","jogador que espera no banco","entra se outro se machucar","fica de fora no inicio"},
    {"R","Cozinha","RAPADURA","bloco de acucar de cana","doce e dura","come com queijo"},
    {"R","Futbol","REAL MADRID","Clube gigante espanhol","CR7","Barcelona"},
    // S
    {"S","Religiao","SANTO","imagem de devocao","fica no oratório","pede graca a ele"},
    {"S","Cozinha","SARAPATEL","feito de miudo de porco","prato nordestino","temperado e forte"},
    {"S","Campo","SILO","guarda graos","grande e redondo","fica na fazenda"},
    // T
    {"T","Cozinha","TOUCINHO","gordura do porco","frita na frigideira","derrete no calor"},
    {"T","Casa","TANQUE","lava roupa","tem esfregao","fica no quintal"},
    {"T","Futebol","TRAVE","barra do gol","a bola bate e volta","chora quem perdeu o gol"},
    // U
    {"U","Cozinha","ULISSES","Guerreiro","Cavalo de Troia","Está na obra Odisseia"},
    {"U","Física","ULTRAVIOLETA","energia eletromagnética invisível","danifica o DNA, causando envelhecimento","é um tom de roxo profundo e vibrante, posicionado no limite superior do espectro visíve"},
    {"U","Ciência","UNICELULAR","Pode ser assexuada (fissão binária) ou sexuada","Sem núcleo organizado","Oque uma célula pode ser"},
    // V
    {"V","Religiao","VELA","acesa para o santo","derrete devagar","clareia o altar"},
    {"V","Cozinha","VIRADO","feito de feijao amassado","com ovo e toucinho","prato mineiro"},
    {"V","Campo","VACA","da leite todo dia","pasta no campo","tem bezerro"},
    // X
    {"X","Química","XENÔNIO","gás nobre","seu símbolo é Xe","usado em lâmpadas de alta intensidade"},
    {"X","Biologia","XILEMA","tecido vegetal","conduz a seiva bruta","transporta água e sais das raízes"},
    {"X","História","XENOFOBIA","preconceito contra estrangeiros","aversão ao que vem de fora","tema recorrente em atualidades"},

    // Z 
    {"Z","Biologia","ZIGOTO","célula ovo","formada na fecundação","primeira etapa do desenvolvimento embrionário"},
    {"Z","História","ZUMBI DOS PALMARES","líder quilombola","símbolo da resistência negra","morreu em 20 de novembro"},
    {"Z","Zoologia","ZOOPLÂNCTON","organismos que flutuam na água","base da cadeia alimentar marinha","composto por pequenos animais e protozoários"},
};
}