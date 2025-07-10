import Image from 'next/image'

import { Button } from '@/shared/ui'

import classes from './NewsItemPreview.module.scss'
import Link from 'next/link'

type NewsItemPreviewProps = {
  id: number
  heading: string
  text: string
}

export const NewsItemPreview = ({ id, heading, text }: NewsItemPreviewProps) => {
  return (
    <div className={classes.newsItemPreview}>
      <h5>{heading}</h5>

      <p>{text}</p>

      <Link href={`/news/${id}`}>
        <Button color='cyan'>Подробнее ›</Button>
      </Link>

      <Image
        className={classes.bgImg}
        src='/placeholders/news-preview.png'
        width={361}
        height={339}
        alt='Картинка'
      />
    </div>
  )
}
