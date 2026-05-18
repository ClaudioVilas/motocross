# 📦 Deployment Guide

## Prerequisites

- GitHub account
- Vercel account (for frontend)
- Render account (for backend)
- Mapbox account (for maps)

## Environment Variables

### Frontend (Vercel)

Create these environment variables in your Vercel project settings:

```
VITE_API_BASE_URL=https://your-backend-url.onrender.com
VITE_SIGNALR_HUB_URL=https://your-backend-url.onrender.com/hubs/tracking
VITE_MAPBOX_TOKEN=your_mapbox_token
```

### Backend (Render)

Create these environment variables in your Render web service:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=(auto-configured by Render PostgreSQL)
Frontend__Url=https://your-frontend-url.vercel.app
```

## Deployment Steps

### 1. Create GitHub Repository

```bash
cd /Users/claudiovilas/Downloads/Proyectos/Motocross

# Already done - repository initialized and committed!
# Now push to GitHub:

# Option A: Using HTTPS (requires Personal Access Token)
git remote set-url origin https://github.com/ClaudioVilas/motocross.git
git push -u origin main

# Option B: Using SSH (recommended)
git remote set-url origin git@github.com:ClaudioVilas/motocross.git
git push -u origin main
```

### 2. Deploy Backend to Render

1. Go to [Render Dashboard](https://dashboard.render.com/)
2. Click "New +" → "Web Service"
3. Connect your GitHub repository
4. Configure:
   - **Name:** motocross-api
   - **Runtime:** .NET
   - **Build Command:** `cd backend && dotnet restore && dotnet build --configuration Release`
   - **Start Command:** `cd backend/src/Api && dotnet run --no-build --configuration Release --urls http://0.0.0.0:$PORT`
   - **Plan:** Free
5. Click "Create PostgreSQL Database" and link it
6. Add environment variables
7. Deploy!

### 3. Deploy Frontend to Vercel

#### Option A: Using Vercel CLI

```bash
cd frontend
npm install -g vercel
vercel login
vercel --prod
```

#### Option B: Using Vercel Dashboard

1. Go to [Vercel Dashboard](https://vercel.com/dashboard)
2. Click "Add New..." → "Project"
3. Import your GitHub repository
4. Configure:
   - **Framework Preset:** Vite
   - **Root Directory:** `frontend`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`
5. Add environment variables
6. Deploy!

### 4. Configure GitHub Secrets

For CI/CD to work, add these secrets in GitHub repository settings:

**Frontend Secrets:**
```
VERCEL_TOKEN=your_vercel_token
VERCEL_ORG_ID=your_org_id
VERCEL_PROJECT_ID=your_project_id
VITE_API_BASE_URL=https://your-backend-url.onrender.com
VITE_SIGNALR_HUB_URL=https://your-backend-url.onrender.com/hubs/tracking
VITE_MAPBOX_TOKEN=your_mapbox_token
```

**Backend Secrets:**
```
RENDER_SERVICE_ID=your_render_service_id
RENDER_API_KEY=your_render_api_key
```

### 5. Update CORS Configuration

After deployment, update backend CORS to include your Vercel URL:

Edit `backend/src/Api/appsettings.json`:
```json
{
  "Frontend": {
    "Url": "https://your-app.vercel.app"
  }
}
```

## Post-Deployment Verification

1. **Backend Health Check:**
   ```bash
   curl https://your-backend.onrender.com/health
   ```

2. **Frontend Check:**
   - Open your Vercel URL
   - Check browser console for errors
   - Test session creation

3. **Real-time Connection:**
   - Create a session
   - Start tracking
   - Verify GPS points are received

## Troubleshooting

### Backend not responding
- Check Render logs
- Verify PostgreSQL connection
- Confirm environment variables are set

### Frontend can't connect to backend
- Verify CORS configuration
- Check API URL environment variable
- Inspect browser network tab

### SignalR connection fails
- Verify WebSocket support on hosting
- Check SignalR hub URL
- Review browser console for errors

### GPS not working
- Ensure HTTPS (required for geolocation)
- Check browser permissions
- Test on actual device (not all browsers support it)

## Production Checklist

- [ ] Backend deployed and accessible
- [ ] Frontend deployed and accessible
- [ ] PostgreSQL database created and connected
- [ ] Environment variables configured
- [ ] CORS properly configured
- [ ] SSL/TLS enabled (HTTPS)
- [ ] GitHub Actions workflows passing
- [ ] GPS permissions working
- [ ] SignalR real-time updates working
- [ ] Create session functionality tested
- [ ] Session tracking tested
- [ ] Lap detection tested

## Monitoring

### Logs

**Render (Backend):**
- Go to your service dashboard
- Click "Logs" tab
- Monitor for errors

**Vercel (Frontend):**
- Go to your deployment
- Click "Functions" → "Logs"
- Check for client-side errors

### Performance

- Monitor Render metrics
- Use Vercel Analytics
- Check database query performance

## Scaling Considerations

### When to Upgrade

**Backend (Render):**
- Free tier has sleep after inactivity
- Upgrade to Starter ($7/month) for 24/7 availability
- Scale PostgreSQL as data grows

**Frontend (Vercel):**
- Free tier is generous for personal projects
- Upgrade if you need:
  - Password protection
  - Advanced analytics
  - Team collaboration

### Database Optimization

- Add indexes on frequently queried fields
- Implement data archival for old sessions
- Consider connection pooling for high traffic

## Backup Strategy

1. **Database Backups:**
   - Render provides automatic backups on paid plans
   - Manual export via pg_dump:
     ```bash
     pg_dump $DATABASE_URL > backup.sql
     ```

2. **Code Backups:**
   - Everything is in Git
   - Regular GitHub backups

## Support

For issues:
1. Check GitHub Issues
2. Review documentation
3. Check Render/Vercel status pages
4. Contact support if needed
