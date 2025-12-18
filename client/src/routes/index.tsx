import { createFileRoute } from '@tanstack/react-router'

import CardStatusCard from '@/components/CardStatusCard'
export const Route = createFileRoute('/')({ component: App })


function App() {
  return (
    <div className="min-h-screen bg-white">
      <section className="relative py-20 px-6 text-center overflow-hidden">
        <div className="relative max-w-5xl mx-auto">
          <CardStatusCard />
        </div>
      </section>
    </div>
  )
}
